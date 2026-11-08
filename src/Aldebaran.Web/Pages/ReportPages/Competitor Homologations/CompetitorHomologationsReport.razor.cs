using Aldebaran.Application.Services.Competitors;
using Aldebaran.Application.Services.Models.Reports;
using Aldebaran.Application.Services.Reports;
using Aldebaran.Web.Pages.ReportPages.Competitor_Homologations.Components;
using Aldebaran.Web.Pages.ReportPages.Competitor_Homologations.ViewModel;
using Aldebaran.Web.Services.Competitors;
using Aldebaran.Infraestructure.Common.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.ReportPages.Competitor_Homologations
{
    /// <summary>
    /// Reporte de Homologaciones con la competencia (Reportes -> Catálogos): árbol Compañía -> Línea -> Artículo -> Homologaciones
    /// con miniatura de la imagen del producto del competidor.
    /// </summary>
    public partial class CompetitorHomologationsReport
    {
        #region Injections
        [Inject]
        protected ILogger<CompetitorHomologationsReport> Logger { get; set; }

        [Inject]
        protected IFileBytesGeneratorService FileBytesGeneratorService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected ICompetitorImageStorage ImageStorage { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected ICompetitorHomologationReportService CompetitorHomologationReportService { get; set; }
        #endregion

        #region Variables
        protected CompetitorHomologationsFilter Filter;
        protected CompetitorHomologationReportResult Report;
        private bool IsLoadingData = false;
        private bool IsBusy = false;
        private bool IsExporting = false;
        protected const string ExcelAction = "xlsx";
        private const string ReportFileName = "Homologaciones con la competencia";
        private const string ExcelMimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
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
            await OpenFilters();
        }

        async Task RedrawReport(CompetitorHomologationReportFilter filter, CancellationToken ct = default)
        {
            try
            {
                IsLoadingData = true;
                Report = await CompetitorHomologationReportService.GetReportAsync(filter, ct);
            }
            finally
            {
                IsLoadingData = false;
                StateHasChanged();
            }
        }

        async Task OpenFilters()
        {
            var result = await DialogService.OpenAsync<CompetitorHomologationsReportFilter>("Filtrar reporte de homologaciones con la competencia", parameters: new Dictionary<string, object> { { "Filter", Filter } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (CompetitorHomologationsFilter)result;
            await RedrawReport(Filter.Criteria);
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
            try
            {
                IsBusy = true;
                var html = await JSRuntime.InvokeAsync<string>("getContent", "competitor-homologations-report-container");
                if (args.Value == "save")
                {
                    // El PDF se arma en el servidor sin URL base: las imágenes se incrustan para que aparezcan.
                    html = await CompetitorReportImageEmbedder.EmbedAsync(html, CompetitorImageStorage.UrlPrefix, name => ImageStorage.ReadAsync(name));
                    var pdfBytes = await FileBytesGeneratorService.GetPdfBytes(html, true);
                    await JSRuntime.InvokeVoidAsync("downloadFile", $"{ReportFileName}.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
                }
                if (args.Value == "print")
                {
                    await JSRuntime.InvokeVoidAsync("print", "competitor-homologations-report-container");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error imprimiendo o generando el PDF del reporte de homologaciones con la competencia");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>Exporta a Excel con el caso de uso de exportación (mismo filtro; una fila por homologación, sin imagen).</summary>
        protected async Task ExportAsync(RadzenSplitButtonItem args)
        {
            var action = args?.Value ?? ExcelAction; // el botón principal también exporta a Excel (única opción)
            if (action != ExcelAction || Filter == null || Report?.HasData != true)
                return;
            try
            {
                IsExporting = true;
                StateHasChanged();
                var rows = await CompetitorHomologationReportService.GetExportAsync(Filter.Criteria);
                var bytes = await FileBytesGeneratorService.GetExcelBytes(rows.ToList());
                await JSRuntime.InvokeVoidAsync("downloadFile", $"{ReportFileName}.xlsx", ExcelMimeType, Convert.ToBase64String(bytes));
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error exportando a Excel el reporte de homologaciones con la competencia");
            }
            finally
            {
                IsExporting = false;
            }
        }
        #endregion
    }
}
