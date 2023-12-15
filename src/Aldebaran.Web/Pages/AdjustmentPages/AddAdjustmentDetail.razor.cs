using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.AdjustmentPages
{
    public partial class AddAdjustmentDetail
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
        public ICollection<AdjustmentDetail> adjustmentDetails { get; set; }

        [Parameter]
        public int ADJUSTMENT_ID { get; set; }

        bool hasREFERENCE_IDValue;

        [Parameter]
        public int REFERENCE_ID { get; set; }

        bool hasWAREHOUSE_IDValue;

        [Parameter]
        public short WAREHOUSE_ID { get; set; }

        #endregion

        #region Global Variables

        protected bool errorVisible;

        protected string alertMessage = "No se ha podido Agregar la Referencia";

        protected AdjustmentDetail adjustmentDetail;

        protected IEnumerable<ItemReference> itemReferencesForREFERENCEID;

        protected IEnumerable<Warehouse> warehousesForWAREHOUSEID;

        public ICollection<ItemReference> references;

        protected bool isSubmitInProgress;

        bool hasADJUSTMENT_IDValue;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {

            itemReferencesForREFERENCEID = await ItemReferenceService.GetAsync("i => i.IS_ACTIVE && i.Item.IS_ACTIVE");

            warehousesForWAREHOUSEID = await WarehouseService.GetAsync();

            adjustmentDetail.Quantity = 0;
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

                if (adjustmentDetails.Any(ad => ad.ReferenceId.Equals(adjustmentDetail.ReferenceId) && ad.WarehouseId.Equals(adjustmentDetail.WarehouseId)))
                    throw new Exception("La Referencia y Bodega seleccionadas, ya existen dentro de este ajuste.");

                adjustmentDetail.Warehouse = await WarehouseService.FindAsync(adjustmentDetail.WarehouseId);

                var reference = await ItemReferenceService.GetAsync($"i=> i.REFERENCE_ID=={adjustmentDetail.ReferenceId}");

                adjustmentDetail.ItemReference = reference.Single();

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

        protected async Task ItemReferenceHandler(ItemReference reference)
        {
            adjustmentDetail.ReferenceId = reference?.ReferenceId ?? 0;
        }

        #endregion
    }
}