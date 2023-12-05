using Aldebaran.Web.Models.AldebaranDb;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.PurchaseOrderPages
{
    public partial class EditPurchaseOrderActivity
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

        [Parameter]
        public int PURCHASE_ORDER_ACTIVITY_ID { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }
        protected bool errorVisible;
        protected Models.AldebaranDb.PurchaseOrderActivity purchaseOrderActivity;
        protected IEnumerable<Models.AldebaranDb.Employee> employees;
        protected bool isSubmitInProgress;
        protected Employee LoggedEmployee { get; set; }

        protected override async Task OnInitializedAsync()
        {
            purchaseOrderActivity = await AldebaranDbService.GetPurchaseOrderActivityByPurchaseOrderActivityId(PURCHASE_ORDER_ACTIVITY_ID);
        }
        protected async Task FormSubmit()
        {
            try
            {
                isSubmitInProgress = true;
                var employee = await AldebaranDbService.GetEmployees(new Query
                {
                    Filter = "i=>i.EMPLOYEE_ID==@0",
                    FilterParameters = new object[] { purchaseOrderActivity.ACTIVITY_EMPLOYEE_ID },
                    Expand = "Area"
                });
                purchaseOrderActivity.ActivityEmployee = employee.Single();
                DialogService.Close(purchaseOrderActivity);
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

        protected async Task EmployeeHandler(Employee employee)
        {
            purchaseOrderActivity.ACTIVITY_EMPLOYEE_ID = employee?.EMPLOYEE_ID ?? 0;
        }
    }
}