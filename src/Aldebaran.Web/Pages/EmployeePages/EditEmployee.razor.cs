using Aldebaran.Web.Models.AldebaranDb;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.EmployeePages
{
    public partial class EditEmployee
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

        [Parameter]
        public int EMPLOYEE_ID { get; set; }

        protected Employee employee;
        protected IEnumerable<Area> areasForAREAID;
        protected IEnumerable<IdentityType> identityTypesForIDENTITYTYPEID;
        protected IEnumerable<Models.ApplicationUser> aplicationUsersForLOGINUSERID;

        protected bool errorVisible;
        protected bool isSubmitInProgress;

        protected override async Task OnInitializedAsync()
        {
            areasForAREAID = await AldebaranDbService.GetAreas();
            identityTypesForIDENTITYTYPEID = await AldebaranDbService.GetIdentityTypes();
            aplicationUsersForLOGINUSERID = await Security.GetUsers();

            employee = await AldebaranDbService.GetEmployeeByEmployeeId(EMPLOYEE_ID);
        }

        protected async Task FormSubmit()
        {
            try
            {
                isSubmitInProgress = true;
                await AldebaranDbService.UpdateEmployee(employee.EMPLOYEE_ID, employee);
                DialogService.Close(true);
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
            finally
            {
                isSubmitInProgress = false;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}