using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using AutoMapper;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.EmployeeAlarmsPages
{
    public partial class EmployeeAlarms
    {
        #region Injections
        [Inject]
        protected ILogger<EmployeeAlarms> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected IMapper Mapper { get; set; }

        [Inject]
        protected IEmployeeService EmployeeService { get; set; }

        [Inject]
        protected IAlarmTypeService AlarmTypeService { get; set; }

        [Inject]
        protected IUsersAlarmTypeService UsersAlarmTypeService { get; set; }

        #endregion

        #region Variables
        protected IEnumerable<AlarmType> AlarmTypes;
        protected RadzenDataGrid<UsersAlarmType> UsersAlarmTypesDataGrid;
        protected string search = "";
        protected bool isLoadingInProgress;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                await GetDataAsync();
            }
            finally
            {
                isLoadingInProgress = false;
            }
        }
        #endregion

        #region Events
        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);

        async Task GetDataAsync(CancellationToken ct = default)
        {
            await Task.Yield();
            AlarmTypes = await AlarmTypeService.GetAsync(ct);
        }
        async Task AddAlarmToEmployee(AlarmType alarmType)
        {
            var result = await DialogService.OpenAsync<AddAlarmToEmployee>("Agregar funcionario a la alarma", new Dictionary<string, object> { { "ALARM_TYPE_ID", alarmType.AlarmTypeId } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            var employeeResult = (List<Employee>)result;
            try
            {
                var usersAlarmTypes = employeeResult.Select(s => new UsersAlarmType
                {
                    AlarmTypeId = alarmType.AlarmTypeId,
                    EmployeeId = s.EmployeeId,
                    Visualize = true,
                    Deactivates = true,
                });
                await UsersAlarmTypeService.AddRangeAsync(usersAlarmTypes);
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Alarma por funcionario",
                    Severity = NotificationSeverity.Success,
                    Detail = $"Funcionario agregado correctamente a la alarma."
                });
                await GetDataAsync();
                await UsersAlarmTypesDataGrid.Reload();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(AddAlarmToEmployee));
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Alarma por funcionario",
                    Detail = $"No se ha podido agregar el funcionario a la alarma."
                });
            }
        }
        async Task OnDeleteEmployee(Employee employee, AlarmType alarmType)
        {
            try
            {
                if (await DialogService.Confirm("Está seguro que desea eliminar este funcionario de la alarma?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar eliminación") == true)
                {
                    await UsersAlarmTypeService.DeleteAsync(alarmType.AlarmTypeId, employee.EmployeeId);
                    await GetDataAsync();
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Alarma por funcionario",
                        Severity = NotificationSeverity.Success,
                        Detail = $"Funcionario eliminado correctamente de la alarma."
                    });
                    await UsersAlarmTypesDataGrid.Reload();
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(OnDeleteEmployee));
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido eliminar el funcionario de la alarma."
                });
            }
        }
        #endregion
    }
}
