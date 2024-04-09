using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.AdjustmentPages
{
    public partial class EditAdjustmentDetail
    {
        #region Injections

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IAdjustmentDetailService AdjustmentDetailService { get; set; }

        [Inject]
        protected IItemReferenceService ItemReferenceService { get; set; }

        [Inject]
        protected IWarehouseService WarehouseService { get; set; }

        #endregion

        #region Parameters

        [Parameter]
        public AdjustmentDetail AdjustmentDetail { get; set; }

        [Parameter]
        public int ADJUSTMENT_ID { get; set; }

        [Parameter]
        public short WAREHOUSE_ID { get; set; }

        [Parameter]
        public int REFERENCE_ID { get; set; }

        #endregion

        #region Global Variables

        bool hasWAREHOUSE_IDValue;

        bool hasREFERENCE_IDValue;

        public AdjustmentDetail adjustmentDetail { get; set; }

        protected bool IsErrorVisible;

        protected string Error;

        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;

        bool hasADJUSTMENT_IDValue;

        protected ItemReference ItemReference { get; set; }

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                adjustmentDetail = new AdjustmentDetail()
                {
                    ItemReference = AdjustmentDetail.ItemReference,
                    Warehouse = AdjustmentDetail.Warehouse,
                    Adjustment = AdjustmentDetail.Adjustment,
                    AdjustmentDetailId = AdjustmentDetail.AdjustmentDetailId,
                    AdjustmentId = AdjustmentDetail.AdjustmentId,
                    Quantity = AdjustmentDetail.Quantity,
                    ReferenceId = AdjustmentDetail.ReferenceId,
                    WarehouseId = AdjustmentDetail.WarehouseId
                };

                ItemReference = await ItemReferenceService.FindAsync(adjustmentDetail.ReferenceId);
            }
            finally
            {
                isLoadingInProgress = false;
            }
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            adjustmentDetail = new AdjustmentDetail() { Adjustment = null, ItemReference = null, Warehouse = null };

            hasADJUSTMENT_IDValue = parameters.TryGetValue<int>("ADJUSTMENT_ID", out var hasADJUSTMENT_IDResult);

            if (hasADJUSTMENT_IDValue)
            {
                adjustmentDetail.AdjustmentId = hasADJUSTMENT_IDResult;
            }

            hasREFERENCE_IDValue = parameters.TryGetValue<int>("REFERENCE_ID", out var hasREFERENCE_IDResult);

            if (hasREFERENCE_IDValue)
            {
                adjustmentDetail.ReferenceId = hasREFERENCE_IDResult;
            }

            hasWAREHOUSE_IDValue = parameters.TryGetValue<short>("WAREHOUSE_ID", out var hasWAREHOUSE_IDResult);

            if (hasWAREHOUSE_IDValue)
            {
                adjustmentDetail.WarehouseId = hasWAREHOUSE_IDResult;
            }
            await base.SetParametersAsync(parameters);
        }

        #endregion

        #region Events

        protected async Task FormSubmit()
        {
            try
            {
                IsErrorVisible = false;
                IsSubmitInProgress = true;
                DialogService.Close(adjustmentDetail);
            }
            catch (Exception ex)
            {
                Error = ex.Message;
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
        }

        #endregion
    }
}