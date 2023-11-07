using Aldebaran.Web.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.EmployeePages
{
    public partial class Employees
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        public AldebaranDbService AldebaranDbService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        protected IEnumerable<Models.AldebaranDb.Employee> employees;
        protected RadzenDataGrid<Models.AldebaranDb.Employee> employeesGrid;
        protected string search = "";
        protected Models.AldebaranDb.Employee employee;
        protected DialogResult dialogResult { get; set; }
        protected bool isLoadingInProgress;

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";
            await employeesGrid.GoToPage(0);
            employees = await AldebaranDbService.GetEmployees(new Query { Filter = $@"i => i.IDENTITY_NUMBER.Contains(@0) || i.DISPLAY_NAME.Contains(@0) || i.FULL_NAME.Contains(@0) || i.Area.AREA_NAME.Contains(@0) || i.Area.AREA_CODE.Contains(@0)", FilterParameters = new object[] { search }, Expand = "Area,IdentityType" });
        }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;

                await Task.Yield();

                employees = await AldebaranDbService.GetEmployees(new Query { Filter = $@"i => i.IDENTITY_NUMBER.Contains(@0) || i.DISPLAY_NAME.Contains(@0) || i.FULL_NAME.Contains(@0) || i.Area.AREA_NAME.Contains(@0) || i.Area.AREA_CODE.Contains(@0)", FilterParameters = new object[] { search }, Expand = "Area,IdentityType" });
            }
            finally
            {
                isLoadingInProgress = false;
            }
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            dialogResult = null;
            var result = await DialogService.OpenAsync<AddEmployee>("Nuevo Funcionario");
            if (result == true)
            {
                dialogResult = new DialogResult { Success = true, Message = "Funcionario creado correctamente." };
            }
            await employeesGrid.Reload();
        }

        protected async Task EditRow(Models.AldebaranDb.Employee args)
        {
            dialogResult = null;
            var result = await DialogService.OpenAsync<EditEmployee>("Actualizar Funcionario", new Dictionary<string, object> { { "EMPLOYEE_ID", args.EMPLOYEE_ID } });
            if (result == true)
            {
                dialogResult = new DialogResult { Success = true, Message = "Funcionario actualizado correctamente." };
            }
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, Models.AldebaranDb.Employee employee)
        {
            try
            {
                dialogResult = null;
                if (await DialogService.Confirm("Está seguro que desea eliminar este funcionario??") == true)
                {
                    var deleteResult = await AldebaranDbService.DeleteEmployee(employee.EMPLOYEE_ID);
                    if (deleteResult != null)
                    {
                        dialogResult = new DialogResult { Success = true, Message = "Funcionario eliminado correctamente." };
                        await employeesGrid.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido eliminar el cliente"
                });
            }
        }
    }
}