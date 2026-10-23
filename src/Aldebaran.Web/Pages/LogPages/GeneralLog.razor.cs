using Aldebaran.Application.Services.GeneralLogs;
using Aldebaran.Application.Services.Models.AlarmLogs;
using Aldebaran.Application.Services.Models.GeneralLogs;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Aldebaran.Web.Pages.LogPages
{
    /// <summary>Log general: mensajes agrupados → ocurrencias → detalle técnico.</summary>
    public partial class GeneralLog
    {
        [Inject] protected IGeneralLogService GeneralLogService { get; set; }

        protected IEnumerable<KeyValuePair<string, string>> levels = GeneralLogLevels.Names.Where(x => x.Key is "Information" or "Warning" or "Error");
        protected IEnumerable<string> sources = Array.Empty<string>();
        protected string level;
        protected string source;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                sources = await GeneralLogService.GetSourcesAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(OnInitializedAsync));
            }
        }

        protected override Task<PagedResult<GeneralLogGroup>> FetchAsync(DateTime from, DateTime to, string searchKey, int skip, int take) =>
            GeneralLogService.GetGroupsAsync(CurrentFilter(from, to, searchKey), skip, take);

        protected async Task SearchTextAsync(string value)
        {
            search = value;
            await ApplyFiltersAsync();
        }

        protected async Task ShowOccurrencesAsync(GeneralLogGroup group) =>
            await DialogService.OpenAsync<GeneralLogOccurrencesDialog>("Ocurrencias del mensaje",
                new Dictionary<string, object>
                {
                    { nameof(GeneralLogOccurrencesDialog.Group), group },
                    { nameof(GeneralLogOccurrencesDialog.Filter), CurrentFilter(dateFrom!.Value, dateTo!.Value, null) }
                },
                new DialogOptions { CloseDialogOnOverlayClick = false, Width = "1000px" });

        /// <summary>El nivel y el origen de la ocurrencia son los del grupo; el texto ya delimitó el grupo.</summary>
        private GeneralLogFilter CurrentFilter(DateTime from, DateTime to, string searchKey) =>
            new(from, to, level, source, searchKey);
    }
}
