using Aldebaran.Application.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.PurchaseOrderPages
{
    public partial class EditPurchaseOrderActivity
    {
        #region Injections
        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        [Inject]
        protected IEmployeeService EmployeeService { get; set; }

        [Inject]
        protected IPurchaseOrderActivityService PurchaseOrderActivityService { get; set; }
        #endregion

        #region Parameters
        [Parameter]
        public int PURCHASE_ORDER_ACTIVITY_ID { get; set; }
        #endregion

        #region Variables
        protected bool IsErrorVisible;
        protected ServiceModel.PurchaseOrderActivity PurchaseOrderActivity;
        protected IEnumerable<ServiceModel.Employee> Employees;
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;
        protected ServiceModel.Employee LoggedEmployee { get; set; }
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                LoggedEmployee = await EmployeeService.FindByLoginUserIdAsync(Security.User.Id);
                PurchaseOrderActivity = await PurchaseOrderActivityService.FindAsync(PURCHASE_ORDER_ACTIVITY_ID);
                Employees = await EmployeeService.GetAsync();
            }
            finally
            {
                isLoadingInProgress = false;
            }

        }
        #endregion

        #region Events
        protected async Task FormSubmit()
        {
            try
            {
                IsSubmitInProgress = true;
                LoggedEmployee = await EmployeeService.FindByLoginUserIdAsync(Security.User.Id);
                PurchaseOrderActivity.EmployeeId = LoggedEmployee.EmployeeId;
                DialogService.Close(PurchaseOrderActivity);
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
        protected async Task EmployeeHandler(ServiceModel.Employee employee)
        {
            PurchaseOrderActivity.ActivityEmployeeId = employee?.EmployeeId ?? 0;
            PurchaseOrderActivity.ActivityEmployee = PurchaseOrderActivity.ActivityEmployeeId == 0 ? null : Employees.Single(s => s.EmployeeId == PurchaseOrderActivity.ActivityEmployeeId);

        }
        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
        #endregion
    }
}