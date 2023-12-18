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
        public IAdjustmentDetailService AdjustmentDetailService { get; set; }

        [Inject]
        public IItemReferenceService ItemReferenceService { get; set; }

        [Inject]
        public IWarehouseService WarehouseService { get; set; }

        #endregion

        #region Parameters

        [Parameter]
        public AdjustmentDetail pAdjustmentDetail { get; set; }

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

        protected bool errorVisible;

        protected string alertMessage;

        protected bool isSubmitInProgress;

        protected IEnumerable<ItemReference> itemReferencesForREFERENCEID;

        protected IEnumerable<Warehouse> warehousesForWAREHOUSEID;

        bool hasADJUSTMENT_IDValue;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            adjustmentDetail = new AdjustmentDetail()
            {
                ItemReference = pAdjustmentDetail.ItemReference,
                Warehouse = pAdjustmentDetail.Warehouse,
                Adjustment = pAdjustmentDetail.Adjustment,
                AdjustmentDetailId = pAdjustmentDetail.AdjustmentDetailId,
                AdjustmentId = pAdjustmentDetail.AdjustmentId,
                Quantity = pAdjustmentDetail.Quantity,
                ReferenceId = pAdjustmentDetail.ReferenceId,
                WarehouseId = pAdjustmentDetail.WarehouseId
            };

            itemReferencesForREFERENCEID = await ItemReferenceService.GetByStatusAsync(true);

            warehousesForWAREHOUSEID = await WarehouseService.GetAsync();
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
                errorVisible = false;
                isSubmitInProgress = true;
                DialogService.Close(adjustmentDetail);
            }
            catch (Exception ex)
            {
                alertMessage = ex.Message;
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

        #endregion
    }
}