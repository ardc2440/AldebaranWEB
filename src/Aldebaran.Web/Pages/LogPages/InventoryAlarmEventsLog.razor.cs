using Aldebaran.Application.Services.AlarmLogs;
using Aldebaran.Application.Services.Models.AlarmLogs;
using Aldebaran.Infraestructure.Common.Utils;
using Aldebaran.Web.Resources.LocalizedControls;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Aldebaran.Web.Pages.LogPages
{
    /// <summary>Consulta de refrescos e inactivaciones de las alarmas de inventario (mínimos, mínimo en bodega local y sin stock).</summary>
    public partial class InventoryAlarmEventsLog
    {
        private const int DefaultDays = 30;

        #region Injections
        [Inject] protected ILogger<InventoryAlarmEventsLog> Logger { get; set; }
        [Inject] protected SecurityService Security { get; set; }
        [Inject] protected IAlarmLogService AlarmLogService { get; set; }
        [Inject] protected ISharedStringLocalizer Localizer { get; set; }
        #endregion

        #region Variables
        protected sealed record EventTypeOption(string Text, InventoryAlarmEventType Value);

        protected readonly IReadOnlyList<EventTypeOption> eventTypes = new[]
        {
            new EventTypeOption("Refresco", InventoryAlarmEventType.Refresh),
            new EventTypeOption("Inactivación", InventoryAlarmEventType.Inactivation)
        };

        protected IReadOnlyList<AlarmLogType> alarmTypes = Array.Empty<AlarmLogType>();
        protected IEnumerable<InventoryAlarmEvent> events;
        protected LocalizedDataGrid<InventoryAlarmEvent> eventsGrid;
        protected string selectedTypeCode;
        protected InventoryAlarmEventType? selectedEventType;
        protected DateTime? dateFrom = DateTime.Today.AddDays(-DefaultDays);
        protected DateTime? dateTo = DateTime.Today;
        protected string search = "";
        protected string errorMessage;
        protected bool isLoading;
        protected int count;
        private int skip;
        private int top = 20;
        #endregion

        #region Lifecycle
        protected override async Task OnInitializedAsync()
        {
            alarmTypes = await AlarmLogService.GetInventoryLogTypesAsync(AlarmLogContextInfo.UserLogRoles(Security));
        }
        #endregion

        #region Events
        protected async Task LoadData(LoadDataArgs args)
        {
            skip = args.Skip ?? 0;
            top = args.Top ?? top;
            await GetEventsAsync();
        }

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";
            await ReloadFromFirstPageAsync();
        }

        protected async Task ApplyFiltersAsync() => await ReloadFromFirstPageAsync();
        #endregion

        #region Presentation
        protected static string EventTypeText(InventoryAlarmEventType eventType) =>
            eventType == InventoryAlarmEventType.Inactivation ? "Inactivación" : "Refresco";

        protected static string ShowChange<T>(T previous, T current) => $"{previous} → {current}";

        protected string FormatDate(DateTime? date) => date?.ToString(Localizer["datetime:format"]) ?? "";
        #endregion

        #region Private
        private async Task ReloadFromFirstPageAsync()
        {
            skip = 0;
            await eventsGrid.FirstPage(true);
        }

        private string ValidateFilters()
        {
            if (dateFrom == null || dateTo == null)
                return "El rango de fechas es obligatorio.";
            if (dateFrom > dateTo)
                return "La fecha inicial no puede ser mayor que la final.";
            return null;
        }

        private async Task GetEventsAsync()
        {
            errorMessage = ValidateFilters();
            if (errorMessage != null)
            {
                events = Array.Empty<InventoryAlarmEvent>();
                count = 0;
                return;
            }

            try
            {
                isLoading = true;
                var filter = new InventoryAlarmLogFilter(dateFrom!.Value, dateTo!.Value, selectedTypeCode, selectedEventType, search);
                var page = await AlarmLogService.GetInventoryAlarmEventsAsync(filter, skip, top);
                events = page.Items;
                count = page.TotalCount;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(GetEventsAsync));
                errorMessage = "No fue posible consultar el log de alarmas.";
            }
            finally
            {
                isLoading = false;
            }
        }
        #endregion
    }
}
