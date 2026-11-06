using Aldebaran.Application.Services.Models.Reports;
using Aldebaran.Application.Services.Reports;
using Aldebaran.Infraestructure.Common.Utils;
using Aldebaran.Web.Pages.ReportPages.Provider_References.Components;
using Aldebaran.Web.Pages.ReportPages.Provider_References.ViewModel;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
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
        /// <summary>Resultado del caso de uso (árbol Proveedor -> Línea -> Artículo -> Referencias -> Bodegas).</summary>
        protected ProviderReferenceReportResult Report;
        private bool IsBusy = false;
        private bool IsLoadingData = false;
        #endregion

        #region Overrides
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await Reset();
            }
        }
        #endregion

        #region Events

        async Task Reset()
        {
            Filter = null;
            Report = null;
            StateHasChanged();
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink", false);
            await OpenFilters();
        }

        async Task RedrawReport(ProviderReferenceReportFilter filter, CancellationToken ct = default)
        {
            try
            {
                IsLoadingData = true;
                Report = await ProviderReferenceReportService.GetReportAsync(filter, ct);
            }
            finally
            {
                IsLoadingData = false;
                StateHasChanged();
            }
        }

        async Task OpenFilters()
        {
            var result = await DialogService.OpenAsync<ProviderReferencesReportFilter>("Filtrar reporte de referencias del proveedor", parameters: new Dictionary<string, object> { { "Filter", Filter } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (ProviderReferencesFilter)result;

            await RedrawReport(ToReportFilter(Filter));

            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink", false);
        }

        /// <summary>Traduce la selección del diálogo al filtro del caso de uso.</summary>
        static ProviderReferenceReportFilter ToReportFilter(ProviderReferencesFilter filter) => new()
        {
            ProviderId = filter.Provider?.ProviderId > 0 ? filter.Provider.ProviderId : null,
            ReferenceIds = filter.ItemReferences.Select(s => s.ReferenceId).ToList()
        };

        async Task RemoveFilters()
        {
            if (await DialogService.Confirm("Está seguro que desea eliminar los filtros establecidos?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar eliminación") == true)
            {
                await Reset();
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

        async Task ToggleReadMorePage()
        {
            await JSRuntime.InvokeVoidAsync("readMoreTogglePage", "toggleLinkPage");
        }

        private async Task ShowImageDialog(string articleName) => DialogService.Open<ImageDialog>("", new Dictionary<string, object>
            {
                { "ArticleName", articleName }
            });
        #endregion
    }
}
