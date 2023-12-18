using Aldebaran.Application.Services;
using Aldebaran.Web.Models;
using Aldebaran.Web.Resources.LocalizedControls;
using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using ViewModels = Aldebaran.Web.Models.ViewModels;

namespace Aldebaran.Web.Pages.EmployeePages
{
    public partial class Employees
    {
        #region Injections
        [Inject]
        protected ILogger<Employees> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        [Inject]
        protected IMapper Mapper { get; set; }

        [Inject]
        protected IEmployeeService EmployeeService { get; set; }
        #endregion

        #region Variables
        protected IEnumerable<ViewModels.EmployeeViewModel> EmployeesList;
        protected LocalizedDataGrid<ViewModels.EmployeeViewModel> EmployeesGrid;
        protected ViewModels.EmployeeViewModel Employee;
        protected string search = "";
        protected DialogResult DialogResult { get; set; }
        protected bool IsLoadingInProgress;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                IsLoadingInProgress = true;
                await GetEmployeesAsync();
            }
            finally
            {
                IsLoadingInProgress = false;
            }
        }
        #endregion

        #region Events
        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";
            await EmployeesGrid.GoToPage(0);
            await GetEmployeesAsync(search);
        }

        async Task GetEmployeesAsync(string searchKey = null, CancellationToken ct = default)
        {
            await Task.Yield();
            var employees = string.IsNullOrEmpty(searchKey) ? await EmployeeService.GetAsync(ct) : await EmployeeService.GetAsync(searchKey, ct);
            EmployeesList = Mapper.Map<List<ViewModels.EmployeeViewModel>>(employees);
            foreach (var employee in EmployeesList)
            {
                employee.ApplicationUser = await Security.GetUserById(employee.LoginUserId);
            }
        }

        protected async Task AddEmployee(MouseEventArgs args)
        {
            DialogResult = null;
            var result = await DialogService.OpenAsync<AddEmployee>("Nuevo Funcionario");
            if (result == true)
            {
                DialogResult = new DialogResult { Success = true, Message = "Funcionario creado correctamente." };
            }
            await GetEmployeesAsync();
            await EmployeesGrid.Reload();
        }

        protected async Task EditEmployee(ViewModels.EmployeeViewModel args)
        {
            DialogResult = null;
            var result = await DialogService.OpenAsync<EditEmployee>("Actualizar Funcionario", new Dictionary<string, object> { { "EMPLOYEE_ID", args.EmployeeId } });
            if (result == true)
            {
                DialogResult = new DialogResult { Success = true, Message = "Funcionario actualizado correctamente." };
            }
            await GetEmployeesAsync();
            await EmployeesGrid.Reload();
        }

        protected async Task DeleteEmployee(MouseEventArgs args, ViewModels.EmployeeViewModel employee)
        {
            try
            {
                DialogResult = null;
                if (await DialogService.Confirm("Está seguro que desea eliminar este funcionario?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar eliminación") == true)
                {
                    await EmployeeService.DeleteAsync(employee.EmployeeId);
                    await GetEmployeesAsync();
                    DialogResult = new DialogResult { Success = true, Message = "Funcionario eliminado correctamente." };
                    await EmployeesGrid.Reload();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(DeleteEmployee));
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido eliminar el cliente"
                });
            }
        }
        #endregion
    }
}