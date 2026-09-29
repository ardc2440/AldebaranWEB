using Aldebaran.Application.FileWritingService.Services.Pdf;
using Aldebaran.Application.FileWritingService.Workers.Inventory;
using Aldebaran.Application.FileWritingService.Workers.Inventory.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using Aldebaran.DataAccess.Infraestructure.Repository.Reports;
using Aldebaran.Infraestructure.Common.Utils;
using Aldebaran.Infraestructure.Core.Ssh;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Aldebaran.Application.FileWritingService.Settings;
using Aldebaran.Application.FileWritingService.Services.Health;
using NCrontab;
using Scriban;
using System.Diagnostics;
using Aldebaran.Application.FileWritingService.Resilience;
using Aldebaran.Application.FileWritingService.Services;
using Aldebaran.DataAccess.Entities;
using System.Net;

namespace Aldebaran.Application.FileWritingService.Workers
{
    internal class InventoryFtpPdfWorker : BackgroundService
    {
        private readonly ILogger<InventoryFtpPdfWorker> _logger;
        private readonly IInventoryReportRepository inventoryReportRepository;
        private readonly IInventoryPdfFileGenerator _pdfFileGenerator;
        private readonly IFtpClient ftpClient;
        private readonly IFtpWritingConnectionRepository ftpWritingConnectionRepository;
        private readonly string TemplatePath;
        private readonly string FileNameTemplate;
        private readonly CrontabSchedule _schedule;
        private DateTime _nextRun;
        private string FileName = string.Empty;
        private readonly ResilientExecutor _executor;
        private readonly IAutomataNotificationRecipientRepository _recipientRepository;
        private readonly Aldebaran.Application.FileWritingService.Services.IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly FtpResilienceOptions _ftpResilienceOptions;
        private readonly ConsecutiveFailureGuard _failureGuard;

        public InventoryFtpPdfWorker(IConfiguration Configuration, ILogger<InventoryFtpPdfWorker> Logger, IInventoryReportRepository InventoryReportRepository, IInventoryPdfFileGenerator PdfFileGenerator, IFtpClient FtpClient, IFtpWritingConnectionRepository ftpWritingConnectionRepository, ResilientExecutor executor, IAutomataNotificationRecipientRepository recipientRepository, Aldebaran.Application.FileWritingService.Services.IEmailSender emailSender, IOptions<FtpResilienceOptions> FtpResilienceOptions, IOptions<FailureGuardOptions> FailureGuardOptions)
        {
            _configuration = Configuration ?? throw new ArgumentNullException(nameof(Configuration));
            inventoryReportRepository = InventoryReportRepository ?? throw new ArgumentNullException(nameof(IInventoryReportRepository));
            _pdfFileGenerator = PdfFileGenerator ?? throw new ArgumentNullException(nameof(IInventoryPdfFileGenerator));
            ftpClient = FtpClient ?? throw new ArgumentNullException(nameof(IFtpClient));
            _logger = Logger ?? throw new ArgumentNullException(nameof(ILogger));
            this.ftpWritingConnectionRepository = ftpWritingConnectionRepository ?? throw new ArgumentNullException(nameof(IFtpWritingConnectionRepository));
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
            _ftpResilienceOptions = FtpResilienceOptions?.Value ?? throw new ArgumentNullException(nameof(FtpResilienceOptions));
            _failureGuard = new ConsecutiveFailureGuard(nameof(InventoryFtpPdfWorker), FailureGuardOptions?.Value?.MaxConsecutiveGenerationFailures ?? 3, Logger);
            _recipientRepository = recipientRepository ?? throw new ArgumentNullException(nameof(IAutomataNotificationRecipientRepository));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(Aldebaran.Application.FileWritingService.Services.IEmailSender));
            TemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "InventoryTemplate.html");
            if (!File.Exists(TemplatePath))
                throw new KeyNotFoundException($"Template file not fount in {TemplatePath}");
            FileNameTemplate = Configuration.GetValue<string>("InventoryFileOutputOptions:Pdf:FileName") ?? throw new KeyNotFoundException("InventoryFileOutputOptions:Pdf:FileName");

            var cronExpression = Configuration.GetValue<string>("InventoryFileOutputOptions:Pdf:CronExpression") ?? throw new KeyNotFoundException("InventoryFileOutputOptions:Pdf:CronExpression");
            _schedule = CrontabSchedule.Parse(cronExpression, new CrontabSchedule.ParseOptions { IncludingSeconds = false });
            var now = DateTime.Now;
            _nextRun = _schedule.GetNextOccurrence(now);
            _logger.LogInformation($"InventoryFtpPdfWorker schedule: {cronExpression} Current Time {now} Next Run: {_nextRun}");
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
                        _logger.LogInformation($"InventoryFtpPdfWorker will be executed at: {now} CorrelationId:{correlationId}");
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();
                        // Do not pre-format FileName; keep template and build per destination respecting overwrite
                        FileName = FileNameTemplate; // keep template for notifications
                        var css = GetCss();

                        // Generate report to temp file
                        string pdfPath = null;
                        try
                        {
                            var htmlTemplate = await GetTemplateHtmlAsync(ct);
                            var html = $"<html><head><meta charset=\"utf-8\"><style>{css}</style></head><body>{htmlTemplate}</body></html>";
                            pdfPath = await _pdfFileGenerator.GenerateAsync(html, true, ct);
                            _failureGuard.RegisterSuccess();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "InventoryFtpPdfWorker report generation failed. CorrelationId:{CorrelationId}", correlationId);
                            try { await SendConnectivityNotification(correlationId, "REPORT_GENERATION", null, 0, FileName, ex.Message, now); } catch (Exception notifyEx) { _logger.LogError(notifyEx, "Error sending report generation notification. CorrelationId:{CorrelationId}", correlationId); }

                            _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
                            stopwatch.Stop();
                            ProcessMemoryLogger.Log(_logger, nameof(InventoryFtpPdfWorker));
                            _failureGuard.RegisterFailure(); // nunca detiene el servicio
                            continue;
                        }

                        // fetch active destinations at execution time
                        IEnumerable<FtpWritingConnection> connections;
                        try
                        {
                            connections = await _executor.ExecuteAsync(async token => await ftpWritingConnectionRepository.GetAllAsync(token), ct);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "InventoryFtpPdfWorker failed reading destinations (DB). CorrelationId:{CorrelationId}", correlationId);
                            try { await SendConnectivityNotification(correlationId, "DB_SOURCE", null, 0, FileName, ex.Message, now); } catch (Exception notifyEx) { _logger.LogError(notifyEx, "Error sending DB connectivity notification. CorrelationId:{CorrelationId}", correlationId); }

                            // Cleanup temp file también cuando falla la lectura de destinos
                            try { if (!string.IsNullOrEmpty(pdfPath) && File.Exists(pdfPath)) File.Delete(pdfPath); } catch { }

                            _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
                            stopwatch.Stop();
                            continue;
                        }

                        var activeConnections = connections.Where(c => c.Active).ToList();

                        // MaxParallelUploads se lee una sola vez (IOptions) en lugar de crear un ConfigurationBuilder por ejecución
                        var maxParallel = Math.Max(1, _ftpResilienceOptions.MaxParallelUploads);

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
                                    var targetFileName = BuildTargetFileName(FileNameTemplate, now, overwrite);

                                    bool uploaded = false;
                                    string uploadError = null;
                                    try
                                    {
                                        // FtpClient reporta los fallos devolviendo false: también se reintenta en ese caso
                                        uploaded = await _executor.ExecuteAsync(async (token) => await ftpClient.UploadFileFromPathAsync(pdfPath, targetFileName, conn.HostName, port, conn.UserName, conn.Password, overwrite, token), retryWhen: result => !result, ct);
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogWarning(ex, "Upload attempt failed for {Host}:{Port} target {File} CorrelationId:{CorrelationId}", conn.HostName, port, targetFileName, correlationId);
                                        uploadError = ex.Message;
                                        uploaded = false;
                                    }

                                    _logger.LogInformation("InventoryFtpPdfWorker uploaded file '{FileName}' to {Host}:{Port} with result {Result} CorrelationId:{CorrelationId}", targetFileName, conn.HostName, port, uploaded, correlationId);

                                    if (!uploaded)
                                    {
                                        try { await SendConnectivityNotification(correlationId, "FTP_DESTINATION", conn.HostName, port, targetFileName, uploadError ?? "Upload failed after retries", now); } catch (Exception ex) { _logger.LogError(ex, "Error sending connectivity notification for FTP destination. CorrelationId:{CorrelationId}", correlationId); }
                                    }
                                }
                                finally
                                {
                                    semaphore.Release();
                                }
                            }).ToList();

                            await Task.WhenAll(uploadTasks);
                        }

                        // Cleanup temp file
                        try { if (!string.IsNullOrEmpty(pdfPath) && File.Exists(pdfPath)) File.Delete(pdfPath); } catch { }

                        _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
                        stopwatch.Stop();
                        _logger.LogInformation($"InventoryFtpPdfWorker has been executed in: {stopwatch.ElapsedMilliseconds} milliseconds | Next Run: {_nextRun} CorrelationId:{correlationId}");
                        ProcessMemoryLogger.Log(_logger, nameof(InventoryFtpPdfWorker));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"InventoryFtpPdfWorker exception {ex.Message}.");
                }
                await Task.Delay(TimeSpan.FromSeconds(10), ct);
            } while (!ct.IsCancellationRequested);
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

        static string? GetCss()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Shared", "css", "print.min.css");
            if (File.Exists(path))
                return File.ReadAllText(path);
            return null;
        }
        async Task<string> GetTemplateHtmlAsync(CancellationToken ct)
        {
            var data = (await inventoryReportRepository.GetInventoryReportDataAsync("", ct)).ToList();
            var model = InventoryPdfModelBuilder.Build(data);
            string htmlTemplate = await File.ReadAllTextAsync(TemplatePath, ct);
            var template = Template.Parse(htmlTemplate);
            var result = template.Render(model);
            return result;
        }

        private static string BuildTargetFileName(string baseFileNameTemplate, DateTime now, bool overwrite)
        {
            // baseFileNameTemplate is expected to be the literal filename configured in appsettings, e.g. "Inventarios_Pdf.pdf"
            // If overwrite == true -> use the filename as-is
            // If overwrite == false -> append _yyyyMMdd before the extension

            var dir = Path.GetDirectoryName(baseFileNameTemplate) ?? string.Empty;
            var fileNameOnly = Path.GetFileName(baseFileNameTemplate);
            var nameOnly = Path.GetFileNameWithoutExtension(fileNameOnly);
            var ext = Path.GetExtension(fileNameOnly);

            // normalize dir to use forward slashes for FTP
            var normalizedDir = dir.Replace('\\', '/').TrimEnd('/');

            string resultFileName;
            if (overwrite)
            {
                resultFileName = fileNameOnly;
            }
            else
            {
                var localNow = now.Kind == DateTimeKind.Utc ? now.ToLocalTime() : now;
                var daily = localNow.ToString("yyyyMMdd");
                resultFileName = $"{nameOnly}_{daily}{ext}";
            }

            if (string.IsNullOrEmpty(normalizedDir))
                return resultFileName;

            return normalizedDir + "/" + resultFileName;
        }
       
    }
}