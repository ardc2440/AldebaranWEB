using Aldebaran.Application.FileWritingService.Workers.Inventory.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using Aldebaran.DataAccess.Infraestructure.Repository.Reports;
using Aldebaran.Infraestructure.Common.Utils;
using Aldebaran.Infraestructure.Core.Ssh;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NCrontab;
using Scriban;
using System.Diagnostics;
namespace Aldebaran.Application.FileWritingService.Workers
{
    internal class InventoryFtpPdfWorker : BackgroundService
    {
        private sealed class FtpScheduleState
        {
            public DataAccess.Entities.FtpWritingConnection Connection { get; set; } = null!;
            public CrontabSchedule Schedule { get; set; } = null!;
            public DateTime NextRun { get; set; }
        }

        private readonly ILogger<InventoryFtpPdfWorker> _logger;
        private readonly IInventoryReportRepository inventoryReportRepository;
        private readonly IFileBytesGeneratorService fileBytesGeneratorService;
        private readonly IFtpClient ftpClient;
        private readonly IFtpWritingConnectionRepository ftpWritingConnectionRepository;
        private readonly string TemplatePath;
        private readonly string FileNameBase;
        private readonly List<FtpScheduleState> _ftpSchedules = new();
        private bool _initialized;
        private string FileName = string.Empty;
        public InventoryFtpPdfWorker(IConfiguration Configuration, ILogger<InventoryFtpPdfWorker> Logger, IInventoryReportRepository InventoryReportRepository, IFileBytesGeneratorService FileBytesGeneratorService, IFtpClient FtpClient, IFtpWritingConnectionRepository ftpWritingConnectionRepository)
        {
            inventoryReportRepository = InventoryReportRepository ?? throw new ArgumentNullException(nameof(IInventoryReportRepository));
            fileBytesGeneratorService = FileBytesGeneratorService ?? throw new ArgumentNullException(nameof(IFileBytesGeneratorService));
            ftpClient = FtpClient ?? throw new ArgumentNullException(nameof(IFtpClient));
            _logger = Logger ?? throw new ArgumentNullException(nameof(ILogger));
            this.ftpWritingConnectionRepository = ftpWritingConnectionRepository ?? throw new ArgumentNullException(nameof(IFtpWritingConnectionRepository));
            TemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "InventoryTemplate.html");
            if (!File.Exists(TemplatePath))
                throw new KeyNotFoundException($"Template file not fount in {TemplatePath}");
            FileNameBase = Configuration.GetValue<string>("InventoryFileOutputOptions:Pdf:FileName") ?? throw new KeyNotFoundException("InventoryFileOutputOptions:Pdf:FileName");
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            do
            {
                try
                {
                    if (!_initialized)
                    {
                        await InitializeSchedulesAsync(ct);
                    }

                    var now = DateTime.Now;
                    var dueConnections = _ftpSchedules.Where(s => now > s.NextRun).ToList();

                    if (dueConnections.Any())
                    {
                        _logger.LogInformation($"InventoryFtpPdfWorker will be executed at: {now} for {dueConnections.Count} connections");
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();
                        FileName = string.Format(FileNameBase, now);
                        var css = GetCss();
                        var htmlTemplate = await GetTemplateHtmlAsync(ct);
                        var html = $"<html><head><style>{css}</style></head><body>{htmlTemplate}</body></html>";
                        var pdfBytes = await fileBytesGeneratorService.GetPdfBytes(html, true);

                        foreach (var state in dueConnections)
                        {
                            var conn = state.Connection;
                            var port = 21;
                            if (!string.IsNullOrWhiteSpace(conn.PortNumber))
                            {
                                int.TryParse(conn.PortNumber, out port);
                            }

                            var overwrite = conn.RewriteFile;
                            var targetFileName = BuildTargetFileName(FileName, now, overwrite);

                            var uploaded = await ftpClient.UploadFileAsync(pdfBytes, targetFileName, conn.HostName, port, conn.UserName, conn.Password, overwrite);
                            _logger.LogInformation($"InventoryFtpPdfWorker uploaded file '{targetFileName}' to {conn.HostName}:{port} with result {uploaded}");

                            state.NextRun = state.Schedule.GetNextOccurrence(now);
                        }

                        stopwatch.Stop();
                        _logger.LogInformation($"InventoryFtpPdfWorker has been executed in: {stopwatch.ElapsedMilliseconds} milliseconds | Next Runs: {string.Join("; ", _ftpSchedules.Select(s => $"{s.Connection.HostName} -> {s.NextRun}"))}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"InventoryFtpPdfWorker exception {ex.Message}.");
                }
                await Task.Delay(TimeSpan.FromSeconds(10), ct);
            } while (!ct.IsCancellationRequested);
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
            var data = await inventoryReportRepository.GetInventoryReportDataAsync("", ct);
            var dLines = data.Select(s => new { s.LineId, s.LineName }).DistinctBy(d => d.LineId).OrderBy(o => o.LineName);
            var model = new InventoryPdfViewModel
            {
                Lines = dLines.Select(line =>
                {
                    var itemsByLine = data.Where(w => w.LineId == line.LineId).Select(s => new { s.ItemId, s.ItemName, s.InternalReference }).DistinctBy(d => d.ItemId).OrderBy(o => o.ItemName);
                    return new InventoryPdfViewModel.Line
                    {
                        LineName = line.LineName,
                        Items = itemsByLine.Select(item =>
                        {
                            var referencesByItem = data.Where(w => w.ItemId == item.ItemId).Select(s => new { s.ReferenceId, s.ReferenceName, s.AvailableAmount, s.FreeZone, s.LocalWarehouse }).DistinctBy(d => d.ReferenceId).OrderBy(o => o.ReferenceName);
                            return new InventoryPdfViewModel.Item
                            {
                                InternalReference = item.InternalReference,
                                ItemName = item.ItemName,
                                References = referencesByItem.Select(reference =>
                                {
                                    var purchaseOrdersByReference = data.Where(w => w.ReferenceId == reference.ReferenceId && w.PurchaseOrderId > 0).Select(s => new { s.PurchaseOrderId, s.OrderDate, s.Warehouse, s.Total }).DistinctBy(d => d.PurchaseOrderId).OrderBy(o => o.OrderDate);
                                    return new InventoryPdfViewModel.Reference
                                    {
                                        ReferenceName = reference.ReferenceName,
                                        AvailableAmount = reference.AvailableAmount,
                                        LocalWarehouse = reference.LocalWarehouse,
                                        FreeZone = reference.FreeZone,
                                        PurchaseOrders = purchaseOrdersByReference.Select(purchaseOrder =>
                                        {
                                            var activitiesByPurchaseOrder = data.Where(w => w.ReferenceId == reference.ReferenceId && w.PurchaseOrderId == purchaseOrder.PurchaseOrderId && w.Description != null && w.Description.Trim().Length > 0);
                                            return new InventoryPdfViewModel.PurchaseOrder
                                            {
                                                Date = purchaseOrder.OrderDate,
                                                Total = purchaseOrder.Total ?? 0,
                                                Warehouse = purchaseOrder.Warehouse,
                                                Activities = activitiesByPurchaseOrder.Select(activity => new InventoryPdfViewModel.Activity
                                                {
                                                    Date = activity.ActivityDate,
                                                    Description = activity.Description
                                                }).ToList()
                                            };
                                        }).ToList()
                                    };
                                }).ToList()
                            };
                        }).ToList()
                    };
                }).ToList()
            };
            string htmlTemplate = await File.ReadAllTextAsync(TemplatePath, ct);
            var template = Template.Parse(htmlTemplate);
            var result = template.Render(model);
            return result;
        }

        private async Task InitializeSchedulesAsync(CancellationToken ct)
        {
            var connections = await ftpWritingConnectionRepository.GetAllAsync(ct);
            var activeConnections = connections.Where(c => c.Active).ToList();
            var now = DateTime.Now;

            foreach (var conn in activeConnections)
            {
                var schedule = CrontabSchedule.Parse(conn.CronoExp, new CrontabSchedule.ParseOptions { IncludingSeconds = false });
                var nextRun = schedule.GetNextOccurrence(now);

                _ftpSchedules.Add(new FtpScheduleState
                {
                    Connection = conn,
                    Schedule = schedule,
                    NextRun = nextRun
                });

                _logger.LogInformation($"InventoryFtpPdfWorker schedule for FTP {conn.HostName}: {conn.CronoExp} Next Run: {nextRun}");
            }

            _initialized = true;
        }

        private static string BuildTargetFileName(string baseFileName, DateTime now, bool overwrite)
        {
            if (overwrite)
            {
                return baseFileName;
            }

            var name = Path.GetFileNameWithoutExtension(baseFileName);
            var ext = Path.GetExtension(baseFileName);
            var timestamp = now.ToString("yyyyMMdd_HHmmss");
            return $"{name}_{timestamp}{ext}";
        }
        //public Task StopAsync(CancellationToken ct)
        //{
        //    return Task.CompletedTask;
        //}
    }
}