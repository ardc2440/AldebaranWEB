using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Pages.ReportPages.Warehouse_Stock.Components;
using Aldebaran.Web.Pages.ReportPages.Warehouse_Stock.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.ReportPages.Warehouse_Stock
{
    public partial class WarehouseStockReport
    {
        #region Injections
        [Inject]
        protected ILogger<WarehouseStockReport> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IPdfService PdfService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected IReferencesWarehouseService ReferencesWarehouseService { get; set; }
        #endregion

        #region Variables
        protected WarehouseStockFilter Filter;
        protected WarehouseStockViewModel ViewModel;
        private bool IsBusy = false;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            ViewModel = new WarehouseStockViewModel()
            {
                Warehouses = new List<WarehouseStockViewModel.Warehouse>()
            };

            ViewModel.Warehouses = (await GetReportWarehousesAsync()).ToList();
        }
        #endregion

        #region Events
        async Task OpenFilters()
        {
            var result = await DialogService.OpenAsync<WarehouseStockReportFilter>("Filtrar reporte de existencias de artículos", parameters: new Dictionary<string, object> { { "Filter", Filter } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (WarehouseStockFilter)result;
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
        async Task Download()
        {
            IsBusy = true;
            var html = await JSRuntime.InvokeAsync<string>("getContent", "warehouse-stock-report-container");
            var pdfBytes = await PdfService.GetBytes(html, true);
            await JSRuntime.InvokeVoidAsync("downloadFile", "Existencias de artículos.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            IsBusy = false;
        }
        async Task ToggleReadMore()
        {
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink");
        }
        #endregion

        #region Fill Data Report

        protected async Task<IEnumerable<WarehouseStockViewModel.Warehouse>> GetReportWarehousesAsync(CancellationToken ct = default)
        {
            var warehousesList = new List<WarehouseStockViewModel.Warehouse>();

            var referenceWarehouses = await ReferencesWarehouseService.GetAllAsync(ct);

            foreach (var warehouse in referenceWarehouses.Select(s => s.Warehouse).DistinctBy(d => d.WarehouseId).OrderBy(o=>o.WarehouseName))
            {
                warehousesList.Add(new WarehouseStockViewModel.Warehouse
                {
                    WarehouseId = warehouse.WarehouseId,
                    WarehouseName = warehouse.WarehouseName,
                    Lines = (await GetReporLinesAsync(referenceWarehouses, warehouse.WarehouseId, ct)).ToList()
                });
            }

            return warehousesList;
        }

        protected async Task<IEnumerable<WarehouseStockViewModel.Line>> GetReporLinesAsync(IEnumerable<ReferencesWarehouse> referenceWarehouses, short warehouseId, CancellationToken ct = default)
        {
            var lines = new List<WarehouseStockViewModel.Line>();

            foreach (var line in referenceWarehouses.Where(w => w.WarehouseId == warehouseId && w.ItemReference.Item.IsActive && w.ItemReference.IsActive).Select(s => s.ItemReference.Item.Line).DistinctBy(l => l.LineId).OrderBy(o=>o.LineName))
            {
                lines.Add(new WarehouseStockViewModel.Line
                {
                    LineName = line.LineName,
                    LineCode = line.LineCode,
                    Items = (await GetReportItemsByLineIdAsync(referenceWarehouses, warehouseId, line.LineId, ct)).ToList()
                });
            }

            return lines;
        }

        protected async Task<IEnumerable<WarehouseStockViewModel.Item>> GetReportItemsByLineIdAsync(IEnumerable<ReferencesWarehouse> referenceWarehouses, short warehouseId, short lineId, CancellationToken ct = default)
        {
            var items = new List<WarehouseStockViewModel.Item>();

            foreach (var item in referenceWarehouses.Where(w => w.ItemReference.Item.LineId == lineId && w.ItemReference.Item.IsActive && w.WarehouseId == warehouseId).Select(s => s.ItemReference.Item).DistinctBy(l => l.ItemId).OrderBy(o=>o.ItemName))
            {
                items.Add(new WarehouseStockViewModel.Item
                {
                    InternalReference = item.InternalReference,
                    ItemName = item.ItemName,
                    References = (await GetReferencesByItemIdAsync(referenceWarehouses, warehouseId, item.ItemId, ct)).ToList()
                });
            }

            return items;
        }

        protected async Task<IEnumerable<WarehouseStockViewModel.Reference>> GetReferencesByItemIdAsync(IEnumerable<ReferencesWarehouse> referenceWarehouses, short warehouseId, int itemId, CancellationToken ct = default)
        {
            var reportReferences = new List<WarehouseStockViewModel.Reference>();

            foreach (var reference in referenceWarehouses.Where(w => w.ItemReference.ItemId == itemId && w.WarehouseId == warehouseId && w.ItemReference.IsActive).OrderBy(o=>o.ItemReference.ReferenceCode))
            {
                reportReferences.Add(new WarehouseStockViewModel.Reference
                {
                    ReferenceName = reference.ItemReference.ReferenceName,
                    AvailableAmount = reference.Quantity,
                    ProviderReferenceName = reference.ItemReference.ProviderReferenceName,
                    ReferenceCode = reference.ItemReference.ReferenceCode                    
                }); ;
            }

            return reportReferences;
        }

        #endregion

    }
}
