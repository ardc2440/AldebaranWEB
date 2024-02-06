using Aldebaran.Application.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.CustomerOrderPages
{
    public partial class CloseCustomerOrderReasonDialog
    {
        #region Injections
        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected ICloseCustomerOrderReasonService CloseCustomerOrderReasonService { get; set; }

        [Inject]
        protected IEmployeeService EmployeeService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }
        #endregion

        #region Parameters

        [Parameter]
        public string TITLE { get; set; }

        #endregion

        #region Variables
        private bool Submitted = false;
        protected bool IsSubmitInProgress;
        private bool IsErrorVisible;
        protected ServiceModel.CloseCustomerOrderReason CloseCustomerOrderReason { get; set; }
        protected IEnumerable<ServiceModel.CloseCustomerOrderReason> CloseCustomerOrderReasons = new List<ServiceModel.CloseCustomerOrderReason>();
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            await Task.Yield();
            CloseCustomerOrderReason = new ServiceModel.CloseCustomerOrderReason();
            CloseCustomerOrderReasons = await CloseCustomerOrderReasonService.GetAsync();
            IsErrorVisible = !CloseCustomerOrderReasons.Any();
        }
        #endregion

        #region Events
        protected async Task FormSubmit()
        {
            try
            {
                IsSubmitInProgress = true;
                Submitted = true;
                if (CloseCustomerOrderReason.CloseCustomerOrderReasonId == 0)
                    return;
                var employee = await EmployeeService.FindByLoginUserIdAsync(Security.User.Id);
                var reason = new ServiceModel.Reason { EmployeeId = employee.EmployeeId, ReasonId = CloseCustomerOrderReason.CloseCustomerOrderReasonId };
                DialogService.Close(reason);
            }
            finally
            {
                IsSubmitInProgress = false;
            }
        }
        protected void Cancel(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
        #endregion
    }
}
