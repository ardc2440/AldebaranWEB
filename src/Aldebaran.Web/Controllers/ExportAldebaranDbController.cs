using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Reports;
using Aldebaran.Web.Utils;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Aldebaran.Web.Controllers
{
    public partial class ExportAldebaranDbController
    {
        private readonly IItemService ItemService;
        private readonly IExportHelper ExportHelper;
        private readonly ICustomerOrderReportService CustomerOrderReportService;
        public ExportAldebaranDbController(ICustomerOrderReportService CustomerOrderReportService, IItemService ItemService, IExportHelper ExportHelper)
        {
            this.ItemService = ItemService;
            this.ExportHelper = ExportHelper;
            this.CustomerOrderReportService = CustomerOrderReportService;
        }

        [HttpGet("/export/AldebaranDb/items/csv")]
        [HttpGet("/export/AldebaranDb/items/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportItemsToCSV(string fileName = null, CancellationToken ct = default)
        {
            var data = await ItemService.GetAsync(ct);
            return ExportHelper.ToCSV(data.ToList(), fileName);
        }

        [HttpGet("/export/AldebaranDb/items/excel")]
        [HttpGet("/export/AldebaranDb/items/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportItemsToExcel(string fileName = null, CancellationToken ct = default)
        {
            var data = await ItemService.GetAsync(ct);
            return ExportHelper.ToExcel(data.ToList(), fileName);
        }

        [HttpGet("/export/AldebaranDb/customer-order/csv")]
        [HttpGet("/export/AldebaranDb/customer-order/csv(fileName='{fileName}',filter='{filter}')")]
        public async Task<FileStreamResult> ExportCustomerOrderToCSV(string fileName = null, string filter = null, CancellationToken ct = default)
        {
            filter = filter == "NoFilter" ? "" : filter;

            var dataReport = await CustomerOrderReportService.GetCustomerOrderExportDataAsync(filter, ct);
                        
            return ExportHelper.ToCSV(dataReport.ToList(), fileName);
        }

        [HttpGet("/export/AldebaranDb/customer-order/excel")]
        [HttpGet("/export/AldebaranDb/customer-order/excel(fileName='{fileName}',filter='{filter}')")]
        public async Task<FileStreamResult> ExportCustomerOrderToExcel(string fileName = null, string filter = null, CancellationToken ct = default)
        {
            filter = filter == "NoFilter" ? "" : filter;
            
            var dataReport = await CustomerOrderReportService.GetCustomerOrderExportDataAsync(filter, ct);
                       
            return ExportHelper.ToExcel(dataReport.ToList(), fileName);
        }

        class CustomerOrderFlatData
        {
            [DisplayName("Nombre del cliente")]
            public string CustomerName { get; set; }
            [DisplayName("Teléfono")]
            public string Phone { get; set; }
            public string Fax { get; set; }
        }
    }
}
