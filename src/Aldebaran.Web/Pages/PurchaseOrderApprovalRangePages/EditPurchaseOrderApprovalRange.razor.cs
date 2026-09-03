using Aldebaran.Application.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.PurchaseOrderApprovalRangePages
{
    public partial class EditPurchaseOrderApprovalRange
    {
        #region Parameters

        [Parameter]
        public int PURCHASE_ORDER_APPROVAL_RANGE_ID { get; set; }

        #endregion

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
        protected string ChangeReason;
        protected ServiceModel.PurchaseOrderApprovalRange PurchaseOrderApprovalRange;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;

                PurchaseOrderApprovalRange = await PurchaseOrderApprovalRangeService.FindAsync(PURCHASE_ORDER_APPROVAL_RANGE_ID) ?? throw new Exception("No se encontró el rango de tolerancia.");

                ChangeReason = string.Empty;
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
                IsErrorVisible = false;
                IsSubmitInProgress = true;

                var employee = await EmployeeService.FindByLoginUserIdAsync(Security.User.Id);

                await PurchaseOrderApprovalRangeService.UpdateAsync(PURCHASE_ORDER_APPROVAL_RANGE_ID, PurchaseOrderApprovalRange, ChangeReason, employee.EmployeeId);

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

        protected async Task CancelButtonClick(
            MouseEventArgs args)
        {
            DialogService.Close(null);

            await Task.CompletedTask;
        }

        #endregion
    }
}