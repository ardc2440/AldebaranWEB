using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using Aldebaran.Web.Data;

namespace Aldebaran.Web.Controllers
{
    public partial class ExportAldebaranDbController : ExportController
    {
        private readonly AldebaranDbContext context;
        private readonly AldebaranDbService service;

        public ExportAldebaranDbController(AldebaranDbContext context, AldebaranDbService service)
        {
            this.service = service;
            this.context = context;
        }

        [HttpGet("/export/AldebaranDb/currencies/csv")]
        [HttpGet("/export/AldebaranDb/currencies/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCurrenciesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetCurrencies(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranDb/currencies/excel")]
        [HttpGet("/export/AldebaranDb/currencies/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCurrenciesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetCurrencies(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranDb/itemreferences/csv")]
        [HttpGet("/export/AldebaranDb/itemreferences/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportItemReferencesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetItemReferences(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranDb/itemreferences/excel")]
        [HttpGet("/export/AldebaranDb/itemreferences/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportItemReferencesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetItemReferences(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranDb/items/csv")]
        [HttpGet("/export/AldebaranDb/items/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportItemsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetItems(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranDb/items/excel")]
        [HttpGet("/export/AldebaranDb/items/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportItemsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetItems(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranDb/itemsareas/csv")]
        [HttpGet("/export/AldebaranDb/itemsareas/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportItemsAreasToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetItemsAreas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranDb/itemsareas/excel")]
        [HttpGet("/export/AldebaranDb/itemsareas/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportItemsAreasToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetItemsAreas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranDb/lines/csv")]
        [HttpGet("/export/AldebaranDb/lines/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportLinesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetLines(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranDb/lines/excel")]
        [HttpGet("/export/AldebaranDb/lines/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportLinesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetLines(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranDb/measureunits/csv")]
        [HttpGet("/export/AldebaranDb/measureunits/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportMeasureUnitsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetMeasureUnits(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranDb/measureunits/excel")]
        [HttpGet("/export/AldebaranDb/measureunits/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportMeasureUnitsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetMeasureUnits(), Request.Query), fileName);
        }
    }
}
