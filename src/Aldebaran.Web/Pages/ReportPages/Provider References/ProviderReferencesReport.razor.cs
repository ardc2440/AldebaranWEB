using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Pages.ReportPages.Provider_References.Component;
using Aldebaran.Web.Pages.ReportPages.Provider_References.ViewModel;
using Aldebaran.Web.Pages.ReportPages.Warehouse_Stock;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.ReportPages.Provider_References
{
    public partial class ProviderReferencesReport
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
        protected IProviderReferenceService ProviderReferenceService { get; set; }

        [Inject]
        protected IReferencesWarehouseService ReferencesWarehouseService { get; set; }
        #endregion

        #region Variables
        protected ProviderReferencesFilter Filter;
        protected ProviderReferencesViewModel ViewModel;
        private bool IsBusy = false;
        protected IEnumerable<ProviderReference> providerReferences { get; set; }
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            providerReferences = await ProviderReferenceService.GetProviderReferecesReport();

            ViewModel = new ProviderReferencesViewModel()
            {
                Providers = await GetProviderReferenceListAsync()
            };
        }
        #endregion

        #region Events

        async Task<List<ProviderReferencesViewModel.Provider>> GetProviderReferenceListAsync(CancellationToken ct = default)
        {
            return (from provider in providerReferences.Select(s => s.Provider).DistinctBy(d => d.ProviderId)
                    select (new ProviderReferencesViewModel.Provider
                    {
                        ContactPerson = provider.ContactPerson,
                        Email = provider.Email,
                        Fax = provider.Fax,
                        Phone = provider.Phone,
                        ProviderAddress = provider.ProviderAddress,
                        ProviderCode = provider.ProviderCode,
                        ProviderName = provider.ProviderName,
                        Lines = GetProviderLines(provider.ProviderId, ct).Result
                    })).ToList();
        }

        async Task<List<ProviderReferencesViewModel.Line>> GetProviderLines(int providerId, CancellationToken ct = default)
        {
            return (from line in providerReferences.Where(w => w.ProviderId == providerId).Select(s => s.ItemReference.Item.Line).DistinctBy(d => d.LineId)
                    select (new ProviderReferencesViewModel.Line
                    {
                        LineCode = line.LineCode,
                        LineName = line.LineName,
                        Items = GetLineItems(providerId, line.LineId, ct).Result
                    })).ToList();
        }

        async Task<List<ProviderReferencesViewModel.Item>> GetLineItems(int providerId, short lineId, CancellationToken ct = default)
        {
            return (from item in providerReferences.Where(w => w.ProviderId == providerId && w.ItemReference.Item.LineId == lineId).Select(s => s.ItemReference.Item).DistinctBy(d => d.ItemId)
                    select (new ProviderReferencesViewModel.Item
                    {
                        InternalReference = item.InternalReference,
                        ItemName = item.ItemName,
                        References = GetItemReferences(providerId, item.ItemId, ct).Result
                    })).ToList();
        }

        async Task<List<ProviderReferencesViewModel.Reference>> GetItemReferences(int providerId, int itemId, CancellationToken ct = default)
        {
            return (from reference in providerReferences.Where(w => w.ProviderId == providerId && w.ItemReference.ItemId == itemId).Select(s => s.ItemReference).DistinctBy(d => d.ReferenceId)
                    select (new ProviderReferencesViewModel.Reference
                    {
                        ReferenceName = reference.ReferenceName,
                        ReferenceCode = reference.ReferenceCode,
                        ProviderReferenceName = reference.ProviderReferenceName,
                        AvailableAmount = reference.InventoryQuantity,
                        ConfirmedAmount = reference.OrderedQuantity,
                        ReservedAmount = reference.ReservedQuantity,
                        Warehouses = GetReferenceWarehouses(reference.ReferenceId, ct)
                    })).ToList();
        }

        async Task<List<ProviderReferencesViewModel.Warehouse>> GetReferenceWarehouses(int referenceId, CancellationToken ct = default)
        {
            return (from warehouseReference in (await ReferencesWarehouseService.GetByReferenceIdAsync(referenceId,ct)).Where(w=>w.Quantity!=0)
                    select (new ProviderReferencesViewModel.Warehouse 
                    { 
                        WarehouseId = warehouseReference.WarehouseId,
                        WarehouseName = warehouseReference.Warehouse.WarehouseName,
                        Amount= warehouseReference.Quantity,
                    })).Tolist();
        }

        async Task OpenFilters()
        {
            var result = await DialogService.OpenAsync<ProviderReferencesReportFilter>("Filtrar reporte de referencias del proveedor", parameters: new Dictionary<string, object> { { "Filter", Filter } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (ProviderReferencesFilter)result;
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
            var html = await JSRuntime.InvokeAsync<string>("getContent", "provider-references-report-container");
            var pdfBytes = await PdfService.GetBytes(html, true);
            await JSRuntime.InvokeVoidAsync("downloadFile", "Referencias del proveedor.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            IsBusy = false;
        }
        async Task ToggleReadMore()
        {
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink");
        }
        #endregion
    }
}
