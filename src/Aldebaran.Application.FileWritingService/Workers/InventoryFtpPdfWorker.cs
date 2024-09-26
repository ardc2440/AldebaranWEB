using Aldebaran.Application.FileWritingService.Workers.Inventory.Models;
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
        private readonly ILogger<InventoryFtpPdfWorker> _logger;
        private readonly IInventoryReportRepository inventoryReportRepository;
        private readonly IFileBytesGeneratorService fileBytesGeneratorService;
        private readonly IFtpClient ftpClient;
        private readonly bool OverwriteExistingFile;
        private readonly CrontabSchedule _schedule;
        private readonly string TemplatePath;
        private readonly string FileNameBase;
        private DateTime _nextRun;
        private string FileName = string.Empty;
        public InventoryFtpPdfWorker(IConfiguration Configuration, ILogger<InventoryFtpPdfWorker> Logger, IInventoryReportRepository InventoryReportRepository, IFileBytesGeneratorService FileBytesGeneratorService, IFtpClient FtpClient)
        {
            inventoryReportRepository = InventoryReportRepository ?? throw new ArgumentNullException(nameof(IInventoryReportRepository));
            fileBytesGeneratorService = FileBytesGeneratorService ?? throw new ArgumentNullException(nameof(IFileBytesGeneratorService));
            ftpClient = FtpClient ?? throw new ArgumentNullException(nameof(IFtpClient));
            _logger = Logger ?? throw new ArgumentNullException(nameof(ILogger));
            TemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "InventoryTemplate.html");
            if (!File.Exists(TemplatePath))
                throw new KeyNotFoundException($"Template file not fount in {TemplatePath}");
            var cronExpression = Configuration.GetValue<string>("InventoryFileOutputOptions:Pdf:CronExpression") ?? throw new KeyNotFoundException("InventoryFileOutputOptions:Pdf:CronExpression");
            FileNameBase = Configuration.GetValue<string>("InventoryFileOutputOptions:Pdf:FileName") ?? throw new KeyNotFoundException("InventoryFileOutputOptions:Pdf:FileName");
            OverwriteExistingFile = Configuration.GetValue<bool>("InventoryFileOutputOptions:Pdf:OverwriteExistingFile");
            _schedule = CrontabSchedule.Parse(cronExpression, new CrontabSchedule.ParseOptions { IncludingSeconds = false });
            var now = DateTime.Now;
            _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
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
                        _logger.LogInformation($"InventoryFtpPdfWorker will be executed at: {now}");
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();
                        FileName = string.Format(FileNameBase, now);
                        var css = GetCss();
                        var htmlTemplate = await GetTemplateHtmlAsync(ct);
                        var html = $"<html><head><style>{css}</style></head><body>{htmlTemplate}</body></html>";
                        var pdfBytes = await fileBytesGeneratorService.GetPdfBytes(html, true);
                        await ftpClient.UploadFileAsync(pdfBytes, FileName, OverwriteExistingFile);
                        _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
                        _logger.LogInformation($"InventoryFtpPdfWorker has been executed in: {stopwatch.ElapsedMilliseconds} milliseconds | Next Run: {_nextRun}");
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
            var data = await inventoryReportRepository.GetInventoryReportDataAsync("2094,2095,2096,2097", ct);
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
                            var referencesByItem = data.Where(w => w.ItemId == item.ItemId).Select(s => new { s.ReferenceId, s.ReferenceName, s.AvailableAmount, s.FreeZone }).DistinctBy(d => d.ReferenceId).OrderBy(o => o.ReferenceName);
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
        //public Task StopAsync(CancellationToken ct)
        //{
        //    return Task.CompletedTask;
        //}
    }
}