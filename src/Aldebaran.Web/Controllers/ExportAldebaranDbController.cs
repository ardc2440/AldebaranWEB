using Aldebaran.Application.Services;
using Aldebaran.Web.Utils;
using Microsoft.AspNetCore.Mvc;

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
    }
}
