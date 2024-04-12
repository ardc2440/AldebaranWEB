using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Resources.LocalizedControls;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.EmployeeAlarmsPages
{
    public partial class AddAlarmToEmployee
    {
        #region Injections
        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IEmployeeService EmployeeService { get; set; }

        [Inject]
        protected IAlarmTypeService AlarmTypeService { get; set; }
        #endregion

        #region Parameters
        [Parameter]
        public short ALARM_TYPE_ID { get; set; }
        #endregion

        #region Variables
        protected AlarmType AlarmType;
        protected bool IsErrorVisible;
        LocalizedDataGrid<ServiceModel.Employee> EmployeeDataGrid;
        protected List<ServiceModel.Employee> Employees;
        protected IList<ServiceModel.Employee> SelectedEmployees = new List<ServiceModel.Employee>();
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;
        protected bool ValidationError = false;
        protected string search = "";
        readonly bool allowRowSelectOnRowClick = true;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                AlarmType = await AlarmTypeService.FindAsync(ALARM_TYPE_ID);
                await GetEmployeesDataAsync();
            }
            finally
            {
                isLoadingInProgress = false;
            }
        }

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";
            await EmployeeDataGrid.GoToPage(0);
            await GetEmployeesDataAsync(search);
        }
        async Task GetEmployeesDataAsync(string searchKey = null, CancellationToken ct = default)
        {
            await Task.Yield();
            var employees = string.IsNullOrEmpty(searchKey) ? await EmployeeService.GetAsync(ct) : await EmployeeService.GetAsync(searchKey, ct);
            var employeesWithLoginAccess = employees.Where(w => w.LoginUserId != null).ToList();
            var alarmEmployees = (await EmployeeService.GetByAlarmTypeAsync(ALARM_TYPE_ID)).ToList();
            var employeesNotIncludedInAlarms = employeesWithLoginAccess.Where(e1 => !alarmEmployees.Any(e2 => e2.EmployeeId == e1.EmployeeId));
            Employees = employeesNotIncludedInAlarms.ToList();
        }
        protected async Task FormSubmit()
        {
            try
            {
                IsSubmitInProgress = true;
                ValidationError = false;
                if (!SelectedEmployees.Any())
                {
                    ValidationError = true;
                    return;
                }
                if (await DialogService.Confirm($"Está seguro que desea agregar los funcionarios seleccionados a la alarma: {AlarmType.Name}?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmación") == true)
                {
                    DialogService.Close(SelectedEmployees.ToList());
                }
            }
            catch (Exception ex)
            {
                IsErrorVisible = true;
            }
            finally
            {
                IsSubmitInProgress = false;
            }
        }
        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
        #endregion
    }
}
