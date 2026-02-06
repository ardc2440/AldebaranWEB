using Aldebaran.Application.FileWritingService.Resilience;
using Aldebaran.Application.FileWritingService.Workers.Inventory.Models;
using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Infraestructure.Repository;
using Aldebaran.DataAccess.Infraestructure.Repository.Reports;
using Aldebaran.Infraestructure.Common.Utils;
using Aldebaran.Infraestructure.Core.Ssh;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NCrontab;
using System.Diagnostics;
using Aldebaran.Application.FileWritingService.Services;
using System.Net;

namespace Aldebaran.Application.FileWritingService.Workers
{
    internal class InventoryFtpExcelWorker : BackgroundService
    {
        private readonly ILogger<InventoryFtpExcelWorker> _logger;
        private readonly IInventoryReportRepository inventoryReportRepository;
        private readonly IFtpClient ftpClient;
        private readonly IFtpWritingConnectionRepository ftpWritingConnectionRepository;
        private readonly IFileBytesGeneratorService fileBytesGeneratorService;
        private readonly string FileNameBase;
        private readonly CrontabSchedule _schedule;
        private DateTime _nextRun;
        private string FileName = string.Empty;
        private readonly ResilientExecutor _executor;
        private readonly IAutomataNotificationRecipientRepository _recipientRepository;
        private readonly Aldebaran.Application.FileWritingService.Services.IEmailSender _emailSender;
        private readonly IConfiguration _configuration;

        public InventoryFtpExcelWorker(IConfiguration Configuration, ILogger<InventoryFtpExcelWorker> Logger, IInventoryReportRepository InventoryReportRepository, IFileBytesGeneratorService FileBytesGeneratorService, IFtpClient FtpClient, IFtpWritingConnectionRepository ftpWritingConnectionRepository, IAutomataNotificationRecipientRepository recipientRepository, Aldebaran.Application.FileWritingService.Services.IEmailSender emailSender, ResilientExecutor executor)
        {
            _configuration = Configuration ?? throw new ArgumentNullException(nameof(Configuration));
            inventoryReportRepository = InventoryReportRepository ?? throw new ArgumentNullException(nameof(IInventoryReportRepository));
            ftpClient = FtpClient ?? throw new ArgumentNullException(nameof(IFtpClient));
            _logger = Logger ?? throw new ArgumentNullException(nameof(ILogger));
            fileBytesGeneratorService = FileBytesGeneratorService ?? throw new ArgumentNullException(nameof(IFileBytesGeneratorService));
            this.ftpWritingConnectionRepository = ftpWritingConnectionRepository ?? throw new ArgumentNullException(nameof(IFtpWritingConnectionRepository));
            _recipientRepository = recipientRepository ?? throw new ArgumentNullException(nameof(IAutomataNotificationRecipientRepository));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(Aldebaran.Application.FileWritingService.Services.IEmailSender));
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
            FileNameBase = Configuration.GetValue<string>("InventoryFileOutputOptions:Excel:FileName") ?? throw new KeyNotFoundException("InventoryFileOutputOptions:Excel:FileName");

            var cronExpression = Configuration.GetValue<string>("InventoryFileOutputOptions:Excel:CronExpression") ?? throw new KeyNotFoundException("InventoryFileOutputOptions:Excel:CronExpression");
            _schedule = CrontabSchedule.Parse(cronExpression, new CrontabSchedule.ParseOptions { IncludingSeconds = false });
            var now = DateTime.Now;
            _nextRun = _schedule.GetNextOccurrence(now);
            _logger.LogInformation($"InventoryFtpExcelWorker schedule: {cronExpression} Current Time {now} Next Run: {_nextRun}");
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            do
            {
                try
                {
                    var now = DateTime.Now;
                    if (now > _nextRun)
                    {
                        var correlationId = Guid.NewGuid().ToString("N");
                        _logger.LogInformation($"InventoryFtpExcelWorker will be executed at: {now} CorrelationId:{correlationId}");
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();
                        FileName = string.Format(FileNameBase, now);

                        // Generate snapshot (report) - handle failures
                        byte[] excelBytes;
                        try
                        {
                            var data = await GetDataAsync(ct);
                            excelBytes = await fileBytesGeneratorService.GetExcelBytes(data);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "InventoryFtpExcelWorker report generation failed. CorrelationId:{CorrelationId}", correlationId);
                            try
                            {
                                await SendConnectivityNotification(correlationId, "REPORT_GENERATION", null, 0, FileName, ex.Message, now);
                            }
                            catch (Exception notifyEx)
                            {
                                _logger.LogError(notifyEx, "Error sending report generation notification. CorrelationId:{CorrelationId}", correlationId);
                            }

                            _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
                            stopwatch.Stop();
                            continue;
                        }

                        // fetch active destinations at execution time (protected)
                        IEnumerable<FtpWritingConnection> connections;
                        try
                        {
                            connections = await _executor.ExecuteAsync(async token => await ftpWritingConnectionRepository.GetAllAsync(token), ct);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "InventoryFtpExcelWorker failed reading destinations (DB). CorrelationId:{CorrelationId}", correlationId);
                            try
                            {
                                await SendConnectivityNotification(correlationId, "DB_SOURCE", null, 0, FileName, ex.Message, now);
                            }
                            catch (Exception notifyEx)
                            {
                                _logger.LogError(notifyEx, "Error sending DB connectivity notification. CorrelationId:{CorrelationId}", correlationId);
                            }

                            _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
                            stopwatch.Stop();
                            continue;
                        }

                        var activeConnections = connections.Where(c => c.Active).ToList();

                        // limit parallelism
                        // Read MaxParallelUploads only from appsettings.json (ignore environment variables)
                        var jsonConfig = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                            .Build();
                        var maxParallel = jsonConfig.GetValue<int>("FtpResilience:MaxParallelUploads", 4);
                        maxParallel = Math.Max(1, maxParallel);

                        using (var semaphore = new SemaphoreSlim(maxParallel))
                        {
                            var uploadTasks = activeConnections.Select(async conn =>
                            {
                                await semaphore.WaitAsync(ct);
                                try
                                {
                                    var port = 21;
                                    if (!string.IsNullOrWhiteSpace(conn.PortNumber))
                                    {
                                        int.TryParse(conn.PortNumber, out port);
                                    }

                                    var overwrite = conn.RewriteFile ?? true;
                                    var targetFileName = BuildTargetFileName(FileName, now, overwrite);

                                    var uploaded = false;
                                    try
                                    {
                                        uploaded = await _executor.ExecuteAsync(async (token) => await ftpClient.UploadFileAsync(excelBytes, targetFileName, conn.HostName, port, conn.UserName, conn.Password), ct);
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogWarning(ex, "Upload attempt failed for {Host}:{Port} target {File} CorrelationId:{CorrelationId}", conn.HostName, port, targetFileName, correlationId);
                                        uploaded = false;
                                    }

                                    _logger.LogInformation("InventoryFtpExcelWorker uploaded file '{FileName}' to {Host}:{Port} with result {Result} CorrelationId:{CorrelationId}", targetFileName, conn.HostName, port, uploaded, correlationId);

                                    if (!uploaded)
                                    {
                                        try
                                        {
                                            await SendConnectivityNotification(correlationId, "FTP_DESTINATION", conn.HostName, port, targetFileName, "Upload failed after retries", now);
                                        }
                                        catch (Exception ex)
                                        {
                                            _logger.LogError(ex, "Error sending connectivity notification for FTP destination. CorrelationId:{CorrelationId}", correlationId);
                                        }
                                    }
                                }
                                finally
                                {
                                    semaphore.Release();
                                }
                            }).ToList();

                            await Task.WhenAll(uploadTasks);
                        }

                        _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
                        stopwatch.Stop();
                        _logger.LogInformation($"InventoryFtpExcelWorker has been executed in: {stopwatch.ElapsedMilliseconds} milliseconds | Next Run: {_nextRun} CorrelationId:{correlationId}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"InventoryFtpExcelWorker exception {ex.Message}.");
                }
                await Task.Delay(TimeSpan.FromSeconds(10), ct);
            } while (!ct.IsCancellationRequested);
        }

        private static string BuildTargetFileName(string baseFileName, DateTime now, bool overwrite)
        {
            if (overwrite)
            {
                return baseFileName;
            }

            var name = Path.GetFileNameWithoutExtension(baseFileName);
            var ext = Path.GetExtension(baseFileName);
            var timestamp = now.ToString("yyyyMMdd");
            return $"{name}_{timestamp}{ext}";
        }

        async Task<List<InventoryExcelViewModel>> GetDataAsync(CancellationToken ct)
        {
            var reportData = await inventoryReportRepository.GetInventoryReportDataAsync("", ct);
            return reportData.Select(s => new InventoryExcelViewModel
            {
                LineId = s.LineId,
                LineName = s.LineName,
                ItemId = s.ItemId,
                ItemName = s.ItemName,
                InternalReference = s.InternalReference,
                ReferenceName = s.ReferenceName,
                AvailableAmount = s.AvailableAmount,
                FreeZone = s.FreeZone,
                PurchaseOrderId = s.PurchaseOrderId,
                ReferenceId = s.ReferenceId,
                OrderDate = s.OrderDate,
                Warehouse = s.Warehouse,
                Total = s.Total,
                ActivityDate = s.ActivityDate,
                Description = s.Description
            }).ToList();
        }

        private async Task SendConnectivityNotification(string correlationId, string failureType, string? host, int port, string fileName, string errorMessage, DateTime snapshotTime)
        {
            try
            {
                var recipients = await _recipientRepository.GetActiveEmailsByTypeAsync("CONNECTIVITY_DOWN");
                if (recipients == null || !recipients.Any())
                {
                    _logger.LogWarning("No recipients configured for CONNECTIVITY_DOWN. CorrelationId:{CorrelationId}", correlationId);
                    return;
                }

                var localNow = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time"));
                var subject = $"[ALERTA] Fallo de conectividad: FileWritingService - {failureType}";
                var hostInfo = host != null ? $"<li>Host: {host}:{port}</li>" : string.Empty;
                var fileInfo = !string.IsNullOrEmpty(fileName) ? $"<li>Archivo: {fileName}</li>" : string.Empty;
                var body = $"<p>Se detectó un fallo en FileWritingService.</p><ul><li>CorrelationId: {correlationId}</li><li>Timestamp: {localNow}</li><li>Tipo de fallo: {failureType}</li>{hostInfo}{fileInfo}<li>Snapshot generado: {snapshotTime}</li><li>Error: {WebUtility.HtmlEncode(errorMessage)}</li></ul>";

                await _emailSender.SendAsync(recipients.ToArray(), subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SendConnectivityNotification failed for CorrelationId:{CorrelationId}", correlationId);
            }
        }
    }
}