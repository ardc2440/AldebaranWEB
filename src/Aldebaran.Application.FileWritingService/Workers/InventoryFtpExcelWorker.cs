using Aldebaran.Application.FileWritingService.Workers.Inventory.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using Aldebaran.DataAccess.Infraestructure.Repository.Reports;
using Aldebaran.Infraestructure.Common.Utils;
using Aldebaran.Infraestructure.Core.Ssh;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NCrontab;
using System.Diagnostics;

namespace Aldebaran.Application.FileWritingService.Workers
{
    internal class InventoryFtpExcelWorker : BackgroundService
    {
        private readonly ILogger<InventoryFtpExcelWorker> _logger;
        private readonly IInventoryReportRepository inventoryReportRepository;
        private readonly IFtpClient ftpClient;
        private readonly IFtpWritingConnectionRepository ftpWritingConnectionRepository;
        private readonly IFileBytesGeneratorService fileBytesGeneratorService;
        private readonly CrontabSchedule _schedule;
        private readonly string FileNameBase;
        private DateTime _nextRun;
        private string FileName = string.Empty;
        private bool OverwriteExistingFile;
        public InventoryFtpExcelWorker(IConfiguration Configuration, ILogger<InventoryFtpExcelWorker> Logger, IInventoryReportRepository InventoryReportRepository, IFileBytesGeneratorService FileBytesGeneratorService, IFtpClient FtpClient, IFtpWritingConnectionRepository ftpWritingConnectionRepository)
        {
            inventoryReportRepository = InventoryReportRepository ?? throw new ArgumentNullException(nameof(IInventoryReportRepository));
            ftpClient = FtpClient ?? throw new ArgumentNullException(nameof(IFtpClient));
            _logger = Logger ?? throw new ArgumentNullException(nameof(ILogger));
            fileBytesGeneratorService = FileBytesGeneratorService ?? throw new ArgumentNullException(nameof(IFileBytesGeneratorService));
            this.ftpWritingConnectionRepository = ftpWritingConnectionRepository ?? throw new ArgumentNullException(nameof(IFtpWritingConnectionRepository));
            var cronExpression = Configuration.GetValue<string>("InventoryFileOutputOptions:Excel:CronExpression") ?? throw new KeyNotFoundException("InventoryFileOutputOptions:Excel:CronExpression");
            FileNameBase = Configuration.GetValue<string>("InventoryFileOutputOptions:Excel:FileName") ?? throw new KeyNotFoundException("InventoryFileOutputOptions:Excel:FileName");
            OverwriteExistingFile = Configuration.GetValue<bool>("InventoryFileOutputOptions:Excel:OverwriteExistingFile");
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
                        _logger.LogInformation($"InventoryFtpExcelWorker will be executed at: {now}");
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();
                        FileName = string.Format(FileNameBase, now);
                        var data = await GetDataAsync(ct);
                        var excelBytes = await fileBytesGeneratorService.GetExcelBytes(data);

                        var connections = await ftpWritingConnectionRepository.GetAllAsync(ct);
                        var activeConnections = connections.Where(c => c.Active).ToList();
                        foreach (var conn in activeConnections)
                        {
                            var port = 21;
                            if (!string.IsNullOrWhiteSpace(conn.PortNumber))
                            {
                                int.TryParse(conn.PortNumber, out port);
                            }

                            var overwrite = conn.RewriteFile;
                            var targetFileName = FileName;
                            if (!overwrite)
                            {
                                var baseName = Path.GetFileNameWithoutExtension(FileName);
                                var ext = Path.GetExtension(FileName);
                                var timestamp = now.ToString("yyyyMMdd_HHmmss");
                                targetFileName = $"{baseName}_{timestamp}{ext}";
                            }

                            var uploaded = await ftpClient.UploadFileAsync(excelBytes, targetFileName, conn.HostName, port, conn.UserName, conn.Password, overwrite);
                            _logger.LogInformation($"InventoryFtpExcelWorker uploaded file '{targetFileName}' to {conn.HostName}:{port} with result {uploaded}");
                        }

                        _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
                        _logger.LogInformation($"InventoryFtpExcelWorker has been executed in: {stopwatch.ElapsedMilliseconds} milliseconds | Next Run: {_nextRun}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"InventoryFtpExcelWorker exception {ex.Message}.");
                }
                await Task.Delay(TimeSpan.FromSeconds(10), ct);
            } while (!ct.IsCancellationRequested);
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
    }
}