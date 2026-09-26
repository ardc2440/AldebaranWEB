using Aldebaran.Application.Services.Models.Reports;
using Aldebaran.Application.Services.Reports;
using Microsoft.AspNetCore.Components;
using System.Diagnostics;

namespace Aldebaran.Web.Pages.ReportPages.Item_References
{
    /// <summary>
    /// Página del reporte de Artículos y Referencias. Solo presenta el resultado del caso de uso;
    /// la consulta, agrupación y orden los resuelve IItemReferencesReportService.
    /// </summary>
    public partial class ItemReferencesReport
    {
        #region Injections
        [Inject]
        protected ILogger<ItemReferencesReport> Logger { get; set; }

        [Inject]
        protected IItemReferencesReportService ItemReferencesReportService { get; set; }
        #endregion

        #region Variables
        protected ItemReferencesReportFilter Filter = new();
        protected ItemReferencesReportResult Report;
        protected bool IsLoadingData;
        protected bool HasError;
        private readonly Stopwatch _renderWatch = new();
        #endregion

        #region Overrides
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadReportAsync();
                return;
            }
            LogRenderTimeIfPending();
        }
        #endregion

        #region Report
        /// <summary>Orquesta la carga: indica progreso, consulta el caso de uso y refresca la vista.</summary>
        protected async Task LoadReportAsync(CancellationToken ct = default)
        {
            try
            {
                SetLoading(true);
                var queryWatch = Stopwatch.StartNew();
                Report = await ItemReferencesReportService.GetReportAsync(Filter, ct);
                Logger.LogInformation("Reporte artículos y referencias: consulta {ElapsedMs} ms, {Items} artículos, {References} referencias",
                    queryWatch.ElapsedMilliseconds, CountItems(), CountReferences());
                _renderWatch.Restart();
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

        private void LogRenderTimeIfPending()
        {
            if (!_renderWatch.IsRunning || IsLoadingData)
                return;
            _renderWatch.Stop();
            Logger.LogInformation("Reporte artículos y referencias: render {ElapsedMs} ms", _renderWatch.ElapsedMilliseconds);
        }

        private int CountItems() => Report?.Lines.Sum(l => l.Items.Count) ?? 0;

        private int CountReferences() => Report?.Lines.Sum(l => l.Items.Sum(i => i.References.Count)) ?? 0;

        private void HandleLoadError(Exception ex)
        {
            HasError = true;
            Report = null;
            Logger.LogError(ex, "Error generando el reporte de artículos y referencias");
        }
        #endregion
    }
}
