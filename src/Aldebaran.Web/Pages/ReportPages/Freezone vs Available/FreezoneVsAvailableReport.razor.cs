using Aldebaran.Application.Services.Reports;
using Aldebaran.Infraestructure.Common.Utils;
using Aldebaran.Web.Pages.ReportPages.Freezone_vs_Available.Components;
using Aldebaran.Web.Pages.ReportPages.Freezone_vs_Available.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.ReportPages.Freezone_vs_Available
{
    public partial class FreezoneVsAvailableReport
    {

        #region Injections
        [Inject]
        protected ILogger<FreezoneVsAvailableReport> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IFileBytesGeneratorService FileBytesGeneratorService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected IFreezoneVsAvailableReportService FreezoneVsAvailableReportService { get; set; }
        #endregion

        #region Variables

        protected FreezoneVsAvailableFilter Filter;
        protected FreezoneVsAvailableViewModel ViewModel;
        private bool IsBusy = false;
        private bool IsLoadingData = false;
        private IEnumerable<Application.Services.Models.Reports.FreezoneVsAvailableReport> DataReport { get; set; }
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
            ViewModel = null;
            StateHasChanged();
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink", false);
            await OpenFilters();
        }
        async Task RedrawReportAsync(string filter = "", CancellationToken ct = default)
        {
            try
            {
                IsLoadingData = true;

                DataReport = await FreezoneVsAvailableReportService.GetFreezoneVsAvailableReportDataAsync(filter, ct);

                ViewModel = new FreezoneVsAvailableViewModel
                {
                    Lines = (await GetLinesAsync(ct)).ToList()
                };
            }
            finally
            {
                IsLoadingData = false;
                StateHasChanged();
            }
        }
        async Task OpenFilters()
        {
            var result = await DialogService.OpenAsync<FreezoneVsAvailableReportFilter>("Filtrar reporte de Zona franca vs. Disponible", parameters: new Dictionary<string, object> { { "Filter", Filter } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (FreezoneVsAvailableFilter)result;

            var referenceIdsFilter = "";

            if (Filter.ItemReferences.Count > 0)
                referenceIdsFilter = String.Join(",", Filter.ItemReferences.Select(s => s.ReferenceId));

            await RedrawReportAsync(referenceIdsFilter);

            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink", false);
        }
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
            var html = await JSRuntime.InvokeAsync<string>("getContent", "freezone-report-container");
            if (args?.Value == "save")
            {
                var pdfBytes = await FileBytesGeneratorService.GetPdfBytes(html, true);
                await JSRuntime.InvokeVoidAsync("downloadFile", "Zona franca vs Disponible.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            }
            if (args?.Value == "print")
            {
                await JSRuntime.InvokeVoidAsync("print", "freezone-report-container");
            }
            IsBusy = false;
        }

        async Task ToggleReadMore()
        {
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink");
        }
        #endregion

        #region Fill Data Report
        protected async Task<IEnumerable<FreezoneVsAvailableViewModel.Line>> GetLinesAsync(CancellationToken ct = default)
        {
            var lines = new List<FreezoneVsAvailableViewModel.Line>();

            foreach (var line in DataReport.Select(s => new { s.LineId, s.LineName, s.LineCode })
                                    .DistinctBy(d => d.LineId)
                                    .OrderBy(o => o.LineName))
            {
                lines.Add(new FreezoneVsAvailableViewModel.Line
                {
                    LineName = line.LineName,
                    LineCode = line.LineCode,
                    Items = (await GetItemsPerLineAsync(line.LineId, ct)).ToList()
                });
            }

            return lines;
        }

        protected async Task<IEnumerable<FreezoneVsAvailableViewModel.Item>> GetItemsPerLineAsync(short lineId, CancellationToken ct = default)
        {
            var items = new List<FreezoneVsAvailableViewModel.Item>();

            foreach (var item in DataReport.Where(w => w.LineId == lineId)
                                    .Select(s => new { s.ItemId, s.ItemName, s.InternalReference })
                                    .DistinctBy(d => d.ItemId)
                                    .OrderBy(o => o.ItemName))
            {
                items.Add(new FreezoneVsAvailableViewModel.Item
                {
                    InternalReference = item.InternalReference,
                    ItemName = item.ItemName,
                    References = (await GetReferencesPerItemAsync(item.ItemId, ct)).ToList()
                });
            }

            return items;
        }

        protected async Task<IEnumerable<FreezoneVsAvailableViewModel.Reference>> GetReferencesPerItemAsync(int itemId, CancellationToken ct = default)
        {
            var inventoryReferences = new List<FreezoneVsAvailableViewModel.Reference>();

            foreach (var reference in DataReport.Where(w => w.ItemId == itemId)
                                        .Select(s => new { s.ReferenceId, s.ReferenceName, s.AvailableAmount, s.FreeZone, s.ReferenceCode })
                                        .DistinctBy(d => d.ReferenceId)
                                        .OrderBy(o => o.ReferenceName))
            {
                inventoryReferences.Add(new FreezoneVsAvailableViewModel.Reference
                {
                    ReferenceName = reference.ReferenceName,
                    AvailableAmount = reference.AvailableAmount,
                    FreeZone = reference.FreeZone,
                    ReferenceCode = reference.ReferenceCode
                });
            }

            return inventoryReferences;
        }

        #endregion

    }
}
