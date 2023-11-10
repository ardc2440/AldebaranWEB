using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.PurchaseOrderPages
{
    public partial class AddPurchaseOrderActivity
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

        protected bool errorVisible;
        protected Models.AldebaranDb.PurchaseOrderActivity purchaseOrderActivity;
        protected IEnumerable<Models.AldebaranDb.Employee> employees;
        protected bool isSubmitInProgress;

        protected override async Task OnInitializedAsync()
        {
            purchaseOrderActivity = new Models.AldebaranDb.PurchaseOrderActivity();
            employees = await AldebaranDbService.GetEmployees();
        }
        protected async Task FormSubmit()
        {
            try
            {
                isSubmitInProgress = true;
                purchaseOrderActivity.ActivityEmployee = await AldebaranDbService.GetEmployeeByEmployeeId(purchaseOrderActivity.ACTIVITY_EMPLOYEE_ID);
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
    }
}