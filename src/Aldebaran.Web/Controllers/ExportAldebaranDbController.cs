using Aldebaran.Web.Data;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("/export/AldebaranDb/customercontacts/csv")]
        [HttpGet("/export/AldebaranDb/customercontacts/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCustomerContactsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetCustomerContacts(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranDb/customercontacts/excel")]
        [HttpGet("/export/AldebaranDb/customercontacts/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCustomerContactsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetCustomerContacts(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranDb/customers/csv")]
        [HttpGet("/export/AldebaranDb/customers/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCustomersToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetCustomers(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranDb/customers/excel")]
        [HttpGet("/export/AldebaranDb/customers/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCustomersToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetCustomers(), Request.Query), fileName);
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

        [HttpGet("/export/AldebaranDb/identitytypes/csv")]
        [HttpGet("/export/AldebaranDb/identitytypes/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportIdentityTypesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetIdentityTypes(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranDb/identitytypes/excel")]
        [HttpGet("/export/AldebaranDb/identitytypes/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportIdentityTypesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetIdentityTypes(), Request.Query), fileName);
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

        [HttpGet("/export/AldebaranDb/shipmentforwarderagentmethods/csv")]
        [HttpGet("/export/AldebaranDb/shipmentforwarderagentmethods/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportShipmentForwarderAgentMethodsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetShipmentForwarderAgentMethods(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranDb/shipmentforwarderagentmethods/excel")]
        [HttpGet("/export/AldebaranDb/shipmentforwarderagentmethods/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportShipmentForwarderAgentMethodsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetShipmentForwarderAgentMethods(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranDb/shipmentmethods/csv")]
        [HttpGet("/export/AldebaranDb/shipmentmethods/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportShipmentMethodsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetShipmentMethods(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranDb/shipmentmethods/excel")]
        [HttpGet("/export/AldebaranDb/shipmentmethods/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportShipmentMethodsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetShipmentMethods(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranDb/shippingmethods/csv")]
        [HttpGet("/export/AldebaranDb/shippingmethods/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportShippingMethodsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetShippingMethods(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranDb/shippingmethods/excel")]
        [HttpGet("/export/AldebaranDb/shippingmethods/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportShippingMethodsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetShippingMethods(), Request.Query), fileName);
        }
    }
}
