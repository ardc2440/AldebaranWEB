using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Pages.ReportPages.Reference_Movement.Components;
using Aldebaran.Web.Pages.ReportPages.Reference_Movement.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.ReportPages.Reference_Movement
{
    public partial class ReferenceMovementReport
    {
        #region Injections
        [Inject]
        protected ILogger<ReferenceMovementReport> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IPdfService PdfService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected IItemReferenceService ItemReferenceService { get; set; }

        [Inject]
        protected IReferencesWarehouseService ReferencesWarehouseService { get; set; }

        #endregion

        #region Variables
        protected ReferenceMovementFilter Filter;
        protected ReferenceMovementViewModel ViewModel;
        private bool IsBusy = false;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {           
            ViewModel = new ReferenceMovementViewModel
            {
                Lines = new List<ReferenceMovementViewModel.Line>()
            };

            ViewModel.Lines = (await GetReporLinesAsync()).ToList();
        }
        #endregion

        #region Events
        async Task OpenFilters()
        {
            var result = await DialogService.OpenAsync<ReferenceMovementReportFilter>("Filtrar reporte de movimientos de artículos", parameters: new Dictionary<string, object> { { "Filter", Filter } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (ReferenceMovementFilter)result;
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
            var html = await JSRuntime.InvokeAsync<string>("getContent", "inventory-movement-report-container");
            var pdfBytes = await PdfService.GetBytes(html, true);
            await JSRuntime.InvokeVoidAsync("downloadFile", "Movimientos de artículos.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            IsBusy = false;
        }
        async Task ToggleReadMore()
        {
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink");
        }
        #endregion

        #region Fill Data Report

        protected async Task<IEnumerable<ReferenceMovementViewModel.Line>> GetReporLinesAsync(CancellationToken ct = default)
        {
            var lines = new List<ReferenceMovementViewModel.Line>();

            var references = await ItemReferenceService.GetReportsReferences(ct:ct);

            foreach (var line in references.Select(s => s.Item.Line).DistinctBy(l => l.LineId).OrderBy(o=>o.LineName))
            {
                lines.Add(new ReferenceMovementViewModel.Line
                {
                    LineName = line.LineName,
                    LineCode = line.LineCode,
                    Items = (await GetReportItemsByLineIdAsync(references, line.LineId, ct)).ToList()
                });
            }

            return lines;
        }

        protected async Task<IEnumerable<ReferenceMovementViewModel.Item>> GetReportItemsByLineIdAsync(IEnumerable<ItemReference> references, short lineId, CancellationToken ct = default)
        {
            var items = new List<ReferenceMovementViewModel.Item>();

            foreach (var item in references.Where(w => w.Item.LineId == lineId).Select(s => s.Item).DistinctBy(l => l.ItemId).OrderBy(o=>o.ItemName))
            {
                items.Add(new ReferenceMovementViewModel.Item
                {
                    InternalReference = item.InternalReference,
                    ItemName = item.ItemName,
                    References = (await GetReferencesByItemIdAsync(references, item.ItemId, ct)).ToList()
                });
            }

            return items;
        }

        protected async Task<IEnumerable<ReferenceMovementViewModel.Reference>> GetReferencesByItemIdAsync(IEnumerable<ItemReference> references, int itemId, CancellationToken ct = default)
        {
            var reportReferences = new List<ReferenceMovementViewModel.Reference>();

            foreach (var reference in references.Where(w => w.ItemId == itemId).OrderBy(o=>o.ReferenceCode))
            {
                reportReferences.Add(new ReferenceMovementViewModel.Reference
                {
                    ReferenceName = reference.ReferenceName,
                    RequestedQuantity = reference.OrderedQuantity,
                    ReservedQuantity = reference.ReservedQuantity,
                    ReferenceCode = reference.ReferenceCode,                    
                    Warehouses = (await GetWarehousesByReferenceIdAsync(reference.ReferenceId, ct)).ToList()

                }); ;
            }

            return reportReferences;
        }

        protected async Task<IEnumerable<ReferenceMovementViewModel.Warehouse>> GetWarehousesByReferenceIdAsync(int referenceId, CancellationToken ct = default)
        {
            var warehouses = new List<ReferenceMovementViewModel.Warehouse>();

            var referenceWarehouses = await ReferencesWarehouseService.GetByReferenceIdAsync(referenceId, ct);

            foreach (var warehouse in referenceWarehouses.OrderBy(o=>o.Warehouse.WarehouseName))
            {
                warehouses.Add(new ReferenceMovementViewModel.Warehouse
                {
                    WarehouseId = warehouse.WarehouseId,
                    Amount = warehouse.Quantity,
                    WarehouseName = warehouse.Warehouse.WarehouseName,
                });

            }

            return warehouses;

        }

        #endregion

    }
}
