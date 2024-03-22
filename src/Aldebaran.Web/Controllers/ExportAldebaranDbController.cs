using Aldebaran.Application.Services;
using Aldebaran.Web.Utils;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Aldebaran.Web.Controllers
{
    public partial class ExportAldebaranDbController : ExportController
    {
        private readonly IItemService ItemService;
        private readonly IExportHelper ExportHelper;
        public ExportAldebaranDbController(IItemService ItemService, IExportHelper ExportHelper)
        {
            this.ItemService = ItemService;
            this.ExportHelper = ExportHelper;
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
        [HttpGet("/export/AldebaranDb/customer-order/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCustomerOrderToCSV(string fileName = null, CancellationToken ct = default)
        {
            //TODO: Obtener informacion del pedido
            var data = new List<CustomerOrderFlatData>() {
                new CustomerOrderFlatData {
                    CustomerName="Javier Linares",
                    Fax="09145821327",
                    Phone="3168849474"
                }
            };
            return ExportHelper.ToCSV(data.ToList(), fileName);
        }

        [HttpGet("/export/AldebaranDb/customer-order/excel")]
        [HttpGet("/export/AldebaranDb/customer-order/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCustomerOrderToExcel(string fileName = null, CancellationToken ct = default)
        {
            //TODO: Obtener informacion del pedido
            var data = new List<CustomerOrderFlatData>() {
                new CustomerOrderFlatData {
                    CustomerName="Javier Linares",
                    Fax="09145821327",
                    Phone="3168849474"
                }
            };
            return ExportHelper.ToExcel(data.ToList(), fileName);
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
