using Aldebaran.Application.Services.Models.Reports;
using Aldebaran.Application.Services.Reports;
using Aldebaran.Infraestructure.Common.Utils;
using Aldebaran.Web.Pages.ReportPages.Provider_References.Components;
using Aldebaran.Web.Pages.ReportPages.Provider_References.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.ReportPages.Provider_References
{
    public partial class ProviderReferencesReport
    {
        #region Injections
        [Inject]
        protected ILogger<ProviderReferencesReport> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IFileBytesGeneratorService FileBytesGeneratorService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected IProviderReferenceReportService ProviderReferenceReportService { get; set; }
        #endregion

        #region Variables
        protected ProviderReferencesFilter Filter;
        protected ProviderReferencesViewModel ViewModel;
        private bool IsBusy = false;
        private bool IsLoadingData = false;
        protected IEnumerable<ProviderReferenceReport> DataReport { get; set; }

        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            await RedrawReport();
        }
        #endregion

        #region Events

        async Task RedrawReport(string filter = "", CancellationToken ct = default)
        {
            try
            {
                IsLoadingData = true;

                DataReport = await ProviderReferenceReportService.GetProviderReferenceReportDataAsync(filter, ct);

                ViewModel = new ProviderReferencesViewModel()
                {
                    Providers = await GetProviderReferenceListAsync()
                };

            }
            finally
            {
                IsLoadingData = false;
            }
        }

        async Task OpenFilters()
        {
            var result = await DialogService.OpenAsync<ProviderReferencesReportFilter>("Filtrar reporte de referencias del proveedor", parameters: new Dictionary<string, object> { { "Filter", Filter } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (ProviderReferencesFilter)result;

            await RedrawReport(await SetReportFilter(Filter));

            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink", false);
        }

        async Task<string> SetReportFilter(ProviderReferencesFilter filter, CancellationToken ct = default)
        {
            var filterResult = string.Empty;

            if (filter.Provider != null)
                if (filter.Provider.ProviderId > 0)
                    filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ProviderId = {filter.Provider.ProviderId}";

            if (filter.ItemReferences.Count > 0)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ReferenceIds = '{String.Join(",", Filter.ItemReferences.Select(s => s.ReferenceId))}'";

            return filterResult;
        }

        async Task RemoveFilters()
        {
            if (await DialogService.Confirm("Está seguro que desea eliminar los filtros establecidos?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar eliminación") == true)
            {
                Filter = null;

                await RedrawReport();

                await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink", false);
            }
        }
        async Task Save(RadzenSplitButtonItem args)
        {
            if (args?.Value == null)
                return;
            IsBusy = true;
            var html = await JSRuntime.InvokeAsync<string>("getContent", "provider-references-report-container");
            if (args?.Value == "save")
            {
                var pdfBytes = await FileBytesGeneratorService.GetPdfBytes(html, true);
                await JSRuntime.InvokeVoidAsync("downloadFile", "Referencias del proveedor.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            }
            if (args?.Value == "print")
            {
                await JSRuntime.InvokeVoidAsync("print", "provider-references-report-container");
            }
            IsBusy = false;
        }
        async Task ToggleReadMore()
        {
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink");
        }

        #endregion

        #region Fill Data Report

        async Task<List<ProviderReferencesViewModel.Provider>> GetProviderReferenceListAsync(CancellationToken ct = default)
        {
            return (from provider in DataReport.Select(s => new { s.ProviderId, s.ContactPerson, s.Email, s.Fax, s.Phone, s.ProviderAddress, s.ProviderCode, s.ProviderName })
                                        .DistinctBy(d => d.ProviderId).OrderBy(o => o.ProviderName)
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
            return (from line in DataReport.Where(w => w.ProviderId == providerId).Select(s => new { s.LineId, s.LineCode, s.LineName }).DistinctBy(s => s.LineId).OrderBy(o => o.LineName)
                    select (new ProviderReferencesViewModel.Line
                    {
                        LineCode = line.LineCode,
                        LineName = line.LineName,
                        Items = GetLineItems(providerId, line.LineId, ct).Result
                    })).ToList();
        }

        async Task<List<ProviderReferencesViewModel.Item>> GetLineItems(int providerId, short lineId, CancellationToken ct = default)
        {
            return (from item in DataReport.Where(w => w.ProviderId == providerId && w.LineId == lineId).Select(s => new { s.ItemId, s.ItemName, s.InternalReference }).DistinctBy(d => d.ItemId).OrderBy(o => o.ItemName)
                    select (new ProviderReferencesViewModel.Item
                    {
                        InternalReference = item.InternalReference,
                        ItemName = item.ItemName,
                        References = GetItemReferences(providerId, item.ItemId, ct).Result
                    })).ToList();
        }

        async Task<List<ProviderReferencesViewModel.Reference>> GetItemReferences(int providerId, int itemId, CancellationToken ct = default)
        {
            return (from reference in DataReport.Where(w => w.ProviderId == providerId && w.ItemId == itemId).Select(s => new { s.ReferenceId, s.ReferenceName, s.ReferenceCode, s.ProviderReferenceName, s.AvailableAmount, s.ConfirmedAmount, s.ReservedAmount })
                                          .DistinctBy(d => d.ReferenceId).OrderBy(o => o.ReferenceName)
                    select (new ProviderReferencesViewModel.Reference
                    {
                        ReferenceName = reference.ReferenceName,
                        ReferenceCode = reference.ReferenceCode,
                        ProviderReferenceName = reference.ProviderReferenceName,
                        AvailableAmount = reference.AvailableAmount,
                        ConfirmedAmount = reference.ConfirmedAmount,
                        ReservedAmount = reference.ReservedAmount,
                        Warehouses = GetReferenceWarehouses(reference.ReferenceId, ct).Result
                    })).ToList();
        }

        async Task<List<ProviderReferencesViewModel.Warehouse>> GetReferenceWarehouses(int referenceId, CancellationToken ct = default)
        {
            var result = (from warehouseReference in DataReport.Where(w => w.ReferenceId == referenceId).Select(s => new { s.WarehouseId, s.WarehouseName, s.Amount })
                                                        .DistinctBy(d => d.WarehouseId).OrderBy(o => o.WarehouseName)
                          select (new ProviderReferencesViewModel.Warehouse
                          {
                              WarehouseId = warehouseReference.WarehouseId,
                              WarehouseName = warehouseReference.WarehouseName,
                              Amount = warehouseReference.Amount,
                          })).ToList();

            return result;
        }

        #endregion
    }
}
