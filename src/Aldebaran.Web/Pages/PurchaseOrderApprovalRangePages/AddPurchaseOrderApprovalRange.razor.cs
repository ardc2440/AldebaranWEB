using Aldebaran.Application.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.PurchaseOrderApprovalRangePages
{
    public partial class AddPurchaseOrderApprovalRange
    {
        #region Injections

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IPurchaseOrderApprovalRangeService PurchaseOrderApprovalRangeService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        [Inject]
        protected IEmployeeService EmployeeService { get; set; }

        #endregion

        #region Variables

        protected bool IsErrorVisible;
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;
        protected string ErrorMessage;
        protected decimal StepValue = 0.01m;
        protected ServiceModel.PurchaseOrderApprovalRange PurchaseOrderApprovalRange;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;

                PurchaseOrderApprovalRange = new ServiceModel.PurchaseOrderApprovalRange { IsActive = true };
            }
            finally
            {
                isLoadingInProgress = false;
            }

            await Task.CompletedTask;
        }

        #endregion

        #region Events

        protected async Task FormSubmit()
        {
            try
            {
                IsSubmitInProgress = true;

                var employee = await EmployeeService.FindByLoginUserIdAsync(Security.User.Id);

                PurchaseOrderApprovalRange.EmployeeId = employee.EmployeeId;
                PurchaseOrderApprovalRange.CreatedDate = DateTime.Now;

                await PurchaseOrderApprovalRangeService.AddAsync(PurchaseOrderApprovalRange);

                DialogService.Close(true);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ocurrió un error al intentar guardar el rango de tolerancia: {ex.Message}";
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

            await Task.CompletedTask;
        }

        #endregion
    }
}