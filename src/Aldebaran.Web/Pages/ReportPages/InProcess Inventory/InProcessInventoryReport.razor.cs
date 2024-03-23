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
        private IEnumerable<Aldebaran.Application.Services.Models.InProcessInventoryReport> DataReport { get; set; }

        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            DataReport = await InProcessInventoryReportService.GetInProcessInventoryReportDataAsync();

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

            foreach (var line in DataReport.Select(s => new { s.LineId, s.LineName, s.LineCode }).DistinctBy(d => d.LineId))
            {
                lines.Add(new InProcessInventoryViewModel.Line
                {
                    LineName = line.LineName,
                    LineCode = line.LineCode,
                    Items = (await GetReportItemsByLineIdAsync(line.LineId, ct)).ToList()
                });
            }

            return lines;
        }

        protected async Task<List<InProcessInventoryViewModel.Item>> GetReportItemsByLineIdAsync(short lineId, CancellationToken ct = default)
        {
            var items = new List<InProcessInventoryViewModel.Item>();

            foreach (var item in DataReport.Where(w=>w.LineId == lineId).Select(s=>new { s.ItemId, s.InternalReference, s.ItemName}).DistinctBy(d=>d.ItemId))
            {
                items.Add(new InProcessInventoryViewModel.Item
                {
                    InternalReference = item.InternalReference,
                    ItemName = item.ItemName,
                    References = (await GetReferencesByItemIdAsync(item.ItemId, ct)).ToList()
                });
            }

            return items;
        }

        protected async Task<List<InProcessInventoryViewModel.Reference>> GetReferencesByItemIdAsync(int itemId, CancellationToken ct = default)
        {
            var reportReferences = new List<InProcessInventoryViewModel.Reference>();

            foreach (var reference in DataReport.Where(w => w.ItemId == itemId).Select(s=> new { s.ReferenceId, s.ReferenceName, s.InProcessAmount}).DistinctBy(d=>d.ReferenceId).OrderBy(o => o.ReferenceName))
            {
                reportReferences.Add(new InProcessInventoryViewModel.Reference
                {
                    ReferenceName = reference.ReferenceName,
                    InProcessAmount = reference.InProcessAmount,
                    Warehouses = (await GetWarehousesByReferenceIdAsync(reference.ReferenceId, ct)).ToList()

                }); ;
            }

            return reportReferences;
        }

        protected async Task<List<InProcessInventoryViewModel.Warehouse>> GetWarehousesByReferenceIdAsync(int referenceId, CancellationToken ct = default)
        {
            var warehouses = new List<InProcessInventoryViewModel.Warehouse>();

            foreach (var warehouse in DataReport.Where(w=>w.ReferenceId == referenceId).Select(s=>new { s.WarehouseId, s.WarehouseName, s.Amount}).OrderBy(o=>o.WarehouseName))
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
