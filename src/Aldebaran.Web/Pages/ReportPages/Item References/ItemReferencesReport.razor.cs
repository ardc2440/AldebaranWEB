using Aldebaran.Application.Services.Models.Reports;
using Aldebaran.Application.Services.Reports;
using Aldebaran.Infraestructure.Common.Utils;
using Aldebaran.Web.Pages.ReportPages.Item_References.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.ReportPages.Item_References
{
    /// <summary>
    /// Página del reporte de Artículos y Referencias. Solo presenta el resultado del caso de uso;
    /// la consulta, agrupación y orden los resuelve IItemReferencesReportService.
    /// La pantalla se pagina por artículos para no cargar el DOM con todo el catálogo.
    /// </summary>
    public partial class ItemReferencesReport
    {
        #region Injections
        [Inject]
        protected ILogger<ItemReferencesReport> Logger { get; set; }

        [Inject]
        protected IItemReferencesReportService ItemReferencesReportService { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IFileBytesGeneratorService FileBytesGeneratorService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected IWebHostEnvironment WebHostEnvironment { get; set; }
        #endregion

        #region Variables
        protected const int PageSize = 50;
        protected const string PagingSummaryFormat = "Página {0} de {1} ({2} artículos)";

        protected const string FilterDialogTitle = "Filtrar reporte de artículos y referencias";

        /// <summary>Filtro aplicado; null = el usuario aún no ha filtrado.</summary>
        protected ItemReferencesReportFilter Filter;
        /// <summary>Descripción legible del filtro aplicado.</summary>
        protected ItemReferencesReportFilterSummary FilterSummary;
        /// <summary>Cambia en cada carga para recrear el paginador y volver a la página 1.</summary>
        protected int LoadVersion;
        protected ItemReferencesReportResult Report;
        protected IReadOnlyList<PageLine> CurrentPageLines = Array.Empty<PageLine>();
        protected const string PrintAction = "print";
        protected const string PdfAction = "save";
        private const string PdfFileName = "Articulos y referencias.pdf";
        protected const string ExcelAction = "xlsx";
        private const string ExcelFileName = "Articulos y referencias.xlsx";
        private const string ExcelMimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        private const string PrintCssPath = "css/print.css";
        /// <summary>A partir de cuántas referencias el documento se considera grande (≈ 110 páginas) y se advierte al usuario.</summary>
        private const int LargeDocumentReferenceThreshold = 1000;

        protected bool IsLoadingData;
        protected bool IsBusy;
        protected bool IsExporting;
        /// <summary>Mensaje visible mientras se genera un documento o exportación; null = no se muestra.</summary>
        protected string ProcessingMessage;
        private const string DocumentProcessingMessage = "Generando el documento, por favor espere. Con muchos artículos puede tardar varios segundos...";
        private const string ExcelProcessingMessage = "Generando el archivo de Excel, por favor espere...";
        protected bool HasError;

        private IReadOnlyList<ReportEntry> _entries = Array.Empty<ReportEntry>();

        protected int TotalItems => _entries.Count;
        #endregion

        #region Overrides
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                await OpenFiltersAsync();
        }
        #endregion

        #region Filters
        /// <summary>Abre el diálogo de filtros; si el usuario confirma, aplica el filtro y recarga el reporte.</summary>
        protected async Task OpenFiltersAsync()
        {
            var result = await DialogService.OpenAsync<ItemReferencesReportFilterDialog>(FilterDialogTitle,
                new Dictionary<string, object> { { nameof(ItemReferencesReportFilterDialog.Filter), Filter } },
                new DialogOptions { Width = "800px" });

            if (result is not ItemReferencesReportFilterResult applied)
                return;

            Filter = applied.Filter;
            FilterSummary = applied.Summary;
            await LoadReportAsync();
        }

        /// <summary>Pide confirmación, deja el reporte sin filtros ni datos y vuelve a abrir el diálogo.</summary>
        protected async Task RemoveFiltersAsync()
        {
            if (await ConfirmRemoveFiltersAsync() != true)
                return;

            ClearReport();
            await OpenFiltersAsync();
        }

        private Task<bool?> ConfirmRemoveFiltersAsync() =>
            DialogService.Confirm("Está seguro que desea eliminar los filtros establecidos?",
                title: "Confirmar eliminación",
                options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" });

        private void ClearReport()
        {
            Filter = null;
            FilterSummary = null;
            Report = null;
            HasError = false;
            _entries = Array.Empty<ReportEntry>();
            CurrentPageLines = Array.Empty<PageLine>();
            StateHasChanged();
        }
        #endregion

        #region Report
        /// <summary>Orquesta la carga: indica progreso, consulta el caso de uso y muestra la primera página.</summary>
        protected async Task LoadReportAsync(CancellationToken ct = default)
        {
            try
            {
                SetLoading(true);
                Report = await ItemReferencesReportService.GetReportAsync(Filter, ct);
                PrepareEntries();
                ResetPaging();
            }
            catch (Exception ex)
            {
                HandleLoadError(ex);
            }
            finally
            {
                SetLoading(false);
            }
        }

        private void SetLoading(bool isLoading)
        {
            IsLoadingData = isLoading;
            if (isLoading)
                HasError = false;
            StateHasChanged();
        }

        private void HandleLoadError(Exception ex)
        {
            HasError = true;
            Report = null;
            _entries = Array.Empty<ReportEntry>();
            CurrentPageLines = Array.Empty<PageLine>();
            Logger.LogError(ex, "Error generando el reporte de artículos y referencias");
        }
        #endregion

        #region Processing feedback
        /// <summary>Marca el estado ocupado, muestra el mensaje y fuerza el repintado antes del trabajo pesado.</summary>
        private async Task ShowProcessingAsync(Action setBusy, string message)
        {
            setBusy();
            ProcessingMessage = message;
            StateHasChanged();
            await Task.Yield();
        }
        #endregion

        #region Totals
        private int CountItems() => Report?.Lines.Sum(l => l.Items.Count) ?? 0;

        private int CountReferences() => Report?.Lines.Sum(l => l.Items.Sum(i => i.References.Count)) ?? 0;
        #endregion

        #region Actions (Imprimir / PDF)
        /// <summary>Orquesta las acciones: arma el documento completo (no la página visible) e imprime o descarga el PDF.</summary>
        protected async Task ExecuteActionAsync(RadzenSplitButtonItem args)
        {
            if (args?.Value == null || Report?.HasData != true)
                return;
            if (!await ConfirmLargeDocumentAsync())
                return;
            try
            {
                await ShowProcessingAsync(() => IsBusy = true, DocumentProcessingMessage);
                var html = await BuildDocumentHtmlAsync();
                if (args.Value == PrintAction)
                    await PrintAsync(html);
                else if (args.Value == PdfAction)
                    await DownloadPdfAsync(html);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error generando el documento del reporte de artículos y referencias ({Action})", args.Value);
            }
            finally
            {
                IsBusy = false;
                ProcessingMessage = null;
            }
        }

        /// <summary>Si el resultado es grande, advierte que el documento puede demorar y sugiere filtrar. true = continuar.</summary>
        private async Task<bool> ConfirmLargeDocumentAsync()
        {
            var references = CountReferences();
            if (references < LargeDocumentReferenceThreshold)
                return true;

            var message = $"El resultado tiene {CountItems():N0} artículos y {references:N0} referencias. " +
                          "Generar el documento puede tardar varios minutos y producir un archivo de gran tamaño. " +
                          "Le recomendamos reducir el resultado aplicando filtros. ¿Desea continuar?";
            return await DialogService.Confirm(message, title: "Documento de gran tamaño",
                options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }) == true;
        }

        private async Task<string> BuildDocumentHtmlAsync()
        {
            var css = await ReadPrintCssAsync();
            var generatedAt = DateTime.Now.ToString(SharedLocalizer["datetime:format"]);
            return ItemReferencesReportHtmlBuilder.Build(Report, FilterSummary, css, generatedAt);
        }

        private async Task<string> ReadPrintCssAsync()
        {
            var path = Path.Combine(WebHostEnvironment.WebRootPath ?? string.Empty, PrintCssPath);
            return File.Exists(path) ? await File.ReadAllTextAsync(path) : string.Empty;
        }

        private async Task PrintAsync(string html) => await JSRuntime.InvokeVoidAsync("printHtml", html);

        private async Task DownloadPdfAsync(string html)
        {
            var pdfBytes = await FileBytesGeneratorService.GetPdfBytes(html, true);
            await JSRuntime.InvokeVoidAsync("downloadFile", PdfFileName, "application/pdf", Convert.ToBase64String(pdfBytes));
        }
        #endregion

        #region Export (Excel)
        /// <summary>
        /// Exporta a Excel con el caso de uso de exportación (filas planas, mismo filtro aplicado).
        /// Se genera en el servidor y se descarga por JS: no depende de la página visible ni lleva el filtro en la URL.
        /// </summary>
        protected async Task ExportAsync(RadzenSplitButtonItem args)
        {
            var action = args?.Value ?? ExcelAction; // el botón principal también exporta a Excel (única opción)
            if (action != ExcelAction || Filter == null || Report?.HasData != true)
                return;
            try
            {
                await ShowProcessingAsync(() => IsExporting = true, ExcelProcessingMessage);
                var rows = await ItemReferencesReportService.GetExportAsync(Filter);
                var bytes = await FileBytesGeneratorService.GetExcelBytes(rows.ToList());
                await JSRuntime.InvokeVoidAsync("downloadFile", ExcelFileName, ExcelMimeType, Convert.ToBase64String(bytes));
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error exportando a Excel el reporte de artículos y referencias");
            }
            finally
            {
                IsExporting = false;
                ProcessingMessage = null;
            }
        }
        #endregion

        #region Paging
        /// <summary>Aplana el árbol a una secuencia ordenada (línea, artículo) sobre la que se pagina.</summary>
        private void PrepareEntries()
        {
            _entries = Report?.Lines
                           .SelectMany(line => line.Items.Select(item => new ReportEntry(line, item)))
                           .ToList()
                       ?? (IReadOnlyList<ReportEntry>)Array.Empty<ReportEntry>();
        }

        protected void OnPageChanged(PagerEventArgs args) => ShowPage(args.Skip);

        /// <summary>Vuelve a la primera página y fuerza la recreación del paginador (nueva carga o nuevo filtro).</summary>
        private void ResetPaging()
        {
            LoadVersion++;
            ShowPage(0);
        }

        /// <summary>Toma los artículos de la página y los reagrupa por línea para pintarlos.</summary>
        private void ShowPage(int skip)
        {
            CurrentPageLines = _entries.Skip(skip)
                                       .Take(PageSize)
                                       .GroupBy(e => e.Line.LineId)
                                       .Select(g => new PageLine(g.First().Line.LineName, g.Select(e => e.Item).ToList()))
                                       .ToList();
        }

        private sealed record ReportEntry(ItemReferencesReportLine Line, ItemReferencesReportItem Item);

        protected sealed record PageLine(string LineName, IReadOnlyList<ItemReferencesReportItem> Items);
        #endregion
    }
}
