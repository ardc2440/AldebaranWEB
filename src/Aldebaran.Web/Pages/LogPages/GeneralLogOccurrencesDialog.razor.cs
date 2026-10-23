using Aldebaran.Application.Services.GeneralLogs;
using Aldebaran.Application.Services.Models.GeneralLogs;
using Aldebaran.Web.Resources.LocalizedControls;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Aldebaran.Web.Pages.LogPages
{
    /// <summary>Ocurrencias de un mensaje agrupado del log general.</summary>
    public partial class GeneralLogOccurrencesDialog
    {
        [Inject] protected ILogger<GeneralLogOccurrencesDialog> Logger { get; set; }
        [Inject] protected DialogService DialogService { get; set; }
        [Inject] protected IGeneralLogService GeneralLogService { get; set; }

        [Parameter] public GeneralLogGroup Group { get; set; }
        [Parameter] public GeneralLogFilter Filter { get; set; }

        protected IEnumerable<GeneralLogOccurrence> items;
        protected LocalizedDataGrid<GeneralLogOccurrence> grid;
        protected string errorMessage;
        protected bool isLoading;
        protected int count;

        protected async Task LoadData(LoadDataArgs args)
        {
            try
            {
                isLoading = true;
                var page = await GeneralLogService.GetOccurrencesAsync(GroupFilter(), Group.GroupKey, args.Skip ?? 0, args.Top ?? 10);
                items = page.Items;
                count = page.TotalCount;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(LoadData));
                errorMessage = "No fue posible consultar las ocurrencias.";
            }
            finally
            {
                isLoading = false;
            }
        }

        protected async Task ShowDetailAsync(long logId) =>
            await DialogService.OpenAsync<GeneralLogDetailDialog>("Detalle técnico",
                new Dictionary<string, object> { { nameof(GeneralLogDetailDialog.LogId), logId } },
                new DialogOptions { CloseDialogOnOverlayClick = true, Width = "1000px" });

        /// <summary>Las ocurrencias se limitan al nivel y origen del grupo (valores técnicos del filtro).</summary>
        private GeneralLogFilter GroupFilter() =>
            Filter with { Level = GeneralLogLevels.Names.FirstOrDefault(x => x.Value == Group.Level).Key ?? Filter.Level, Source = Group.Source };
    }
}
