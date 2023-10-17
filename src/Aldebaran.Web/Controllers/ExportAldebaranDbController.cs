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

        [HttpGet("/export/AldebaranDb/areas/csv")]
        [HttpGet("/export/AldebaranDb/areas/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAreasToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAreas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranDb/areas/excel")]
        [HttpGet("/export/AldebaranDb/areas/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAreasToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAreas(), Request.Query), fileName);
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

        [HttpGet("/export/AldebaranDb/cities/csv")]
        [HttpGet("/export/AldebaranDb/cities/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCitiesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetCities(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranDb/cities/excel")]
        [HttpGet("/export/AldebaranDb/cities/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCitiesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetCities(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranDb/countries/csv")]
        [HttpGet("/export/AldebaranDb/countries/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCountriesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetCountries(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranDb/countries/excel")]
        [HttpGet("/export/AldebaranDb/countries/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCountriesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetCountries(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranDb/departments/csv")]
        [HttpGet("/export/AldebaranDb/departments/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportDepartmentsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetDepartments(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranDb/departments/excel")]
        [HttpGet("/export/AldebaranDb/departments/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportDepartmentsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetDepartments(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranDb/forwarders/csv")]
        [HttpGet("/export/AldebaranDb/forwarders/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportForwardersToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetForwarders(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranDb/forwarders/excel")]
        [HttpGet("/export/AldebaranDb/forwarders/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportForwardersToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetForwarders(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranDb/forwarderagents/csv")]
        [HttpGet("/export/AldebaranDb/forwarderagents/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportForwarderAgentsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetForwarderAgents(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranDb/forwarderagents/excel")]
        [HttpGet("/export/AldebaranDb/forwarderagents/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportForwarderAgentsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetForwarderAgents(), Request.Query), fileName);
        }
    }
}
