using Aldebaran.Application.Services;
using Aldebaran.Web.Pages.ReportPages.InProcess_Inventory.Components;
using Aldebaran.Web.Pages.ReportPages.InProcess_Inventory.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.ReportPages.InProcess_Inventory
{
    public partial class InProcessInventoryReport
    {
        #region Injections
        [Inject]
        protected ILogger<InProcessInventoryReport> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IPdfService PdfService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected IInProcessInventoryReportService InProcessInventoryReportService { get; set; }

        #endregion

        #region Variables
        protected InProcessInventoryFilter Filter;
        protected InProcessInventoryViewModel ViewModel;
        List<InProcessInventoryViewModel.Warehouse> UniqueWarehouses = new List<InProcessInventoryViewModel.Warehouse>();
        private bool IsBusy = false;
        private IEnumerable<Aldebaran.Application.Services.Models.InProcessInventoryReport> dataReport;

        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            dataReport = await InProcessInventoryReportService.GetInProcessInventoryReportDataAsync();

            ViewModel = new InProcessInventoryViewModel
            {
                Lines = await GetReporLinesAsync()
            };

            UniqueWarehouses = ViewModel.Lines.SelectMany(s => s.Items)
                .SelectMany(item => item.References.SelectMany(reference => reference.Warehouses))
                .DistinctBy(w => w.WarehouseId)
                .ToList();
        }
        #endregion

        #region Events
        async Task OpenFilters()
        {
            var result = await DialogService.OpenAsync<InProcessInventoryReportFilter>("Filtrar reporte de inventario", parameters: new Dictionary<string, object> { { "Filter", Filter } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (InProcessInventoryFilter)result;
            //Todo: Aplicar filtro de refenrecias al ViewModel
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink", false);
        }
        async Task RemoveFilters()
        {
            if (await DialogService.Confirm("Está seguro que desea eliminar los filtros establecidos?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar eliminación") == true)
            {
                Filter = null;
                //Todo: Remover filtro de refenrecias al ViewModel
                await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink", false);
            }
        }
        async Task Download(MouseEventArgs args)
        {
            IsBusy = true;
            var html = await JSRuntime.InvokeAsync<string>("getContent", "inprocess-inventory-report-container");
            var pdfBytes = await PdfService.GetBytes(html, true);
            await JSRuntime.InvokeVoidAsync("downloadFile", "Inventario en proceso.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            IsBusy = false;
        }
        async Task ToggleReadMore()
        {
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink");
        }

        #endregion

        #region Fill Data Report

        protected async Task<List<InProcessInventoryViewModel.Line>> GetReporLinesAsync(CancellationToken ct = default)
        {
            var lines = new List<InProcessInventoryViewModel.Line>();

            foreach (var line in dataReport.Select(s => new { s.LineName, s.LineCode }).DistinctBy(d => d.LineCode))
            {
                lines.Add(new InProcessInventoryViewModel.Line
                {
                    LineName = line.LineName,
                    LineCode = line.LineCode,
                    Items = (await GetReportItemsByLineIdAsync(line.LineCode, ct)).ToList()
                });
            }

            return lines;
        }

        protected async Task<List<InProcessInventoryViewModel.Item>> GetReportItemsByLineIdAsync(string lineCode, CancellationToken ct = default)
        {
            var items = new List<InProcessInventoryViewModel.Item>();

            foreach (var item in dataReport.Where(w=>w.LineCode.Equals(lineCode)).Select(s=>new { s.InternalReference, s.ItemName}).DistinctBy(d=>d.ItemName))
            {
                items.Add(new InProcessInventoryViewModel.Item
                {
                    InternalReference = item.InternalReference,
                    ItemName = item.ItemName,
                    References = (await GetReferencesByItemIdAsync(item.ItemName, ct)).ToList()
                });
            }

            return items;
        }

        protected async Task<List<InProcessInventoryViewModel.Reference>> GetReferencesByItemIdAsync(string itemName, CancellationToken ct = default)
        {
            var reportReferences = new List<InProcessInventoryViewModel.Reference>();

            foreach (var reference in dataReport.Where(w => w.ItemName.Equals(itemName)).Select(s=> new { s.ReferenceName, s.InProcessAmount}).DistinctBy(d=>d.ReferenceName).OrderBy(o => o.ReferenceName))
            {
                reportReferences.Add(new InProcessInventoryViewModel.Reference
                {
                    ReferenceName = reference.ReferenceName,
                    InProcessAmount = reference.InProcessAmount,
                    Warehouses = (await GetWarehousesByReferenceIdAsync(reference.ReferenceName, ct)).ToList()

                }); ;
            }

            return reportReferences;
        }

        protected async Task<List<InProcessInventoryViewModel.Warehouse>> GetWarehousesByReferenceIdAsync(string referenceName, CancellationToken ct = default)
        {
            var warehouses = new List<InProcessInventoryViewModel.Warehouse>();

            foreach (var warehouse in dataReport.Where(w=>w.ReferenceName.Equals(referenceName)).Select(s=>new { s.WarehouseId, s.WarehouseName, s.Amount}).OrderBy(o=>o.WarehouseName))
            {
                warehouses.Add(new InProcessInventoryViewModel.Warehouse
                {
                    WarehouseId = warehouse.WarehouseId,
                    Amount = warehouse.Amount,
                    WarehouseName = warehouse.WarehouseName,
                });

            }

            return warehouses;

        }

        #endregion


    }
}
