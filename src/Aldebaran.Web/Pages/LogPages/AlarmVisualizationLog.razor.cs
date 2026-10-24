using Aldebaran.Application.Services.AlarmLogs;
using Aldebaran.Application.Services.Models.AlarmLogs;
using Aldebaran.Web.Resources.LocalizedControls;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Aldebaran.Web.Pages.LogPages
{
    /// <summary>
    /// Consulta genérica de visualización de alarmas activas por contexto (órdenes, pedidos, reservas, inventario).
    /// El contexto llega en la URL; el usuario debe tener el rol de log de ese contexto.
    /// Solo se listan alarmas visualizadas; al expandir una alarma se ven los empleados que la visualizaron.
    /// </summary>
    public partial class AlarmVisualizationLog
    {
        private const int DefaultDays = 30;

        #region Injections
        [Inject] protected ILogger<AlarmVisualizationLog> Logger { get; set; }
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [Inject] protected SecurityService Security { get; set; }
        [Inject] protected IAlarmLogService AlarmLogService { get; set; }
        #endregion

        #region Parameters
        [Parameter] public string ContextRoute { get; set; }
        #endregion

        #region Variables
        protected AlarmLogContextInfo contextInfo;
        protected IReadOnlyList<AlarmLogType> alarmTypes = Array.Empty<AlarmLogType>();
        protected IEnumerable<AlarmLogItem> alarms;
        protected IReadOnlyList<AlarmLogVisualizer> visualizers = Array.Empty<AlarmLogVisualizer>();
        protected IReadOnlyList<AlarmVisualization> visualizations = Array.Empty<AlarmVisualization>();
        protected int? selectedEmployeeId;
        protected bool isLoadingVisualizations;
        protected LocalizedDataGrid<AlarmLogItem> alarmsGrid;
        protected string selectedTypeCode;
        protected DateTime? dateFrom;
        protected DateTime? dateTo;
        protected string search = "";
        protected string errorMessage;
        protected bool isLoading;
        protected int count;
        private int skip;
        private int top = 20;
        #endregion

        #region Lifecycle
        protected override async Task OnParametersSetAsync()
        {
            contextInfo = ResolveAuthorizedContext();
            if (contextInfo == null)
            {
                NavigationManager.NavigateTo("unauthorized");
                return;
            }

            ResetFilters();
            await LoadTypesAsync();
            if (alarmsGrid != null)
                await ReloadFromFirstPageAsync();
        }
        #endregion

        #region Events
        protected async Task LoadData(LoadDataArgs args)
        {
            skip = args.Skip ?? 0;
            top = args.Top ?? top;
            await GetAlarmsAsync();
        }

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";
            await ReloadFromFirstPageAsync();
        }

        protected async Task ApplyFiltersAsync() => await ReloadFromFirstPageAsync();

        /// <summary>Al cambiar el tipo de alarma se recargan los empleados del filtro (el seleccionado puede no aplicar).</summary>
        protected async Task ChangeTypeAsync()
        {
            selectedEmployeeId = null;
            await LoadVisualizersAsync();
            await ReloadFromFirstPageAsync();
        }

        /// <summary>Subgrilla de la alarma expandida: empleados que la visualizaron.</summary>
        protected async Task LoadVisualizationsAsync(AlarmLogItem alarm)
        {
            try
            {
                isLoadingVisualizations = true;
                visualizations = Array.Empty<AlarmVisualization>();
                visualizations = await AlarmLogService.GetVisualizationsAsync(selectedTypeCode, alarm.AlarmKey);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(LoadVisualizationsAsync));
                errorMessage = "No fue posible consultar quién visualizó la alarma.";
            }
            finally
            {
                isLoadingVisualizations = false;
            }
        }
        #endregion

        #region Private
        private AlarmLogContextInfo ResolveAuthorizedContext()
        {
            var info = AlarmLogContextInfo.FromRoute(ContextRoute);
            return info != null && Security.IsInRole(AlarmLogContextInfo.AdministratorRole, info.Role) ? info : null;
        }

        private void ResetFilters()
        {
            dateTo = DateTime.Today;
            dateFrom = DateTime.Today.AddDays(-DefaultDays);
            search = "";
            selectedEmployeeId = null;
            errorMessage = null;
            alarms = null;
            count = 0;
        }

        private async Task LoadTypesAsync()
        {
            alarmTypes = await AlarmLogService.GetTypesAsync(contextInfo!.Context, AlarmLogContextInfo.UserLogRoles(Security));
            selectedTypeCode = alarmTypes.FirstOrDefault()?.Code;
            await LoadVisualizersAsync();
        }

        private async Task LoadVisualizersAsync()
        {
            visualizers = string.IsNullOrEmpty(selectedTypeCode)
                ? Array.Empty<AlarmLogVisualizer>()
                : await AlarmLogService.GetVisualizersAsync(selectedTypeCode);
        }

        private async Task ReloadFromFirstPageAsync()
        {
            skip = 0;
            if (alarmsGrid != null)
                await alarmsGrid.FirstPage(true);
            else
                await GetAlarmsAsync();
        }

        private string ValidateFilters()
        {
            if (string.IsNullOrEmpty(selectedTypeCode))
                return "Seleccione el tipo de alarma.";
            if (dateFrom == null || dateTo == null)
                return "El rango de fechas es obligatorio.";
            if (dateFrom > dateTo)
                return "La fecha inicial no puede ser mayor que la final.";
            return null;
        }

        private async Task GetAlarmsAsync()
        {
            errorMessage = ValidateFilters();
            if (errorMessage != null)
            {
                alarms = Array.Empty<AlarmLogItem>();
                count = 0;
                return;
            }

            try
            {
                isLoading = true;
                var page = await AlarmLogService.GetAlarmsAsync(selectedTypeCode, dateFrom!.Value, dateTo!.Value, search, selectedEmployeeId, skip, top);
                alarms = page.Items;
                count = page.TotalCount;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(GetAlarmsAsync));
                errorMessage = "No fue posible consultar las alarmas.";
            }
            finally
            {
                isLoading = false;
            }
        }
        #endregion
    }
}
