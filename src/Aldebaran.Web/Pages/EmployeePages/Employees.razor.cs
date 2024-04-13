using Aldebaran.Application.Services;
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
        protected TooltipService TooltipService { get; set; }

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
        protected bool isLoadingInProgress;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                await GetEmployeesAsync();
            }
            finally
            {
                isLoadingInProgress = false;
            }
        }
        #endregion

        #region Events
        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);

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
            var result = await DialogService.OpenAsync<AddEmployee>("Nuevo Funcionario");
            if (result == true)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Funcionario",
                    Severity = NotificationSeverity.Success,
                    Detail = $"Funcionario creado correctamente."
                });
            }
            await GetEmployeesAsync(search);
            await EmployeesGrid.Reload();
        }

        protected async Task EditEmployee(ViewModels.EmployeeViewModel args)
        {
            var result = await DialogService.OpenAsync<EditEmployee>("Actualizar Funcionario", new Dictionary<string, object> { { "EMPLOYEE_ID", args.EmployeeId } });
            if (result == true)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Funcionario",
                    Severity = NotificationSeverity.Success,
                    Detail = $"Funcionario actualizado correctamente."
                });
            }
            await GetEmployeesAsync(search);
            await EmployeesGrid.Reload();
        }

        protected async Task DeleteEmployee(MouseEventArgs args, ViewModels.EmployeeViewModel employee)
        {
            try
            {
                if (await DialogService.Confirm("Está seguro que desea eliminar este funcionario?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar eliminación") == true)
                {
                    await EmployeeService.DeleteAsync(employee.EmployeeId);
                    await GetEmployeesAsync(search);
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Funcionario",
                        Severity = NotificationSeverity.Success,
                        Detail = $"Funcionario eliminado correctamente."
                    });
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
                    Detail = $"No se ha podido eliminar el funcionario."
                });
            }
        }
        #endregion
    }
}