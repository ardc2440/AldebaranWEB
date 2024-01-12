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
        protected IAdjustmentDetailService AdjustmentDetailService { get; set; }

        [Inject]
        protected IItemReferenceService ItemReferenceService { get; set; }

        [Inject]
        protected IWarehouseService WarehouseService { get; set; }

        #endregion

        #region Parameters

        [Parameter]
        public ICollection<AdjustmentDetail> AdjustmentDetails { get; set; }

        [Parameter]
        public int AdjustmentId { get; set; }

        [Parameter]
        public int ReferenceId { get; set; }

        [Parameter]
        public short WarehouseId { get; set; }

        #endregion

        #region Properties

        protected IEnumerable<ItemReference> ItemReferencesForReferenceId { get; set; } = new List<ItemReference>();
        protected IEnumerable<Warehouse> WarehousesForWarehouseId { get; set; } = new List<Warehouse>();

        #endregion

        #region Global Variables

        protected bool errorVisible;
        protected string alertMessage = "No se ha podido agregar la referencia";
        protected AdjustmentDetail adjustmentDetail;
        protected bool isSubmitInProgress;
        bool hasAdjustmentIdValue;
        bool hasWarehouseIdValue;
        bool hasReferenceIdValue;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {

            ItemReferencesForReferenceId = await ItemReferenceService.GetByStatusAsync(true);

            WarehousesForWarehouseId = await WarehouseService.GetAsync();

            adjustmentDetail.Quantity = 0;
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            adjustmentDetail = new AdjustmentDetail() { Adjustment = null, ItemReference = null, Warehouse = null };

            hasAdjustmentIdValue = parameters.TryGetValue<int>("AdjustmentId", out var hasADJUSTMENT_IDResult);

            if (hasAdjustmentIdValue)
            {
                adjustmentDetail.AdjustmentId = hasADJUSTMENT_IDResult;
            }

            hasReferenceIdValue = parameters.TryGetValue<int>("ReferenceId", out var hasREFERENCE_IDResult);

            if (hasReferenceIdValue)
            {
                adjustmentDetail.ReferenceId = hasREFERENCE_IDResult;
            }

            hasWarehouseIdValue = parameters.TryGetValue<short>("WarehouseId", out var hasWAREHOUSE_IDResult);

            if (hasWarehouseIdValue)
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

                if (AdjustmentDetails.Any(ad => ad.ReferenceId.Equals(adjustmentDetail.ReferenceId) && ad.WarehouseId.Equals(adjustmentDetail.WarehouseId)))
                    throw new Exception("La Referencia y Bodega seleccionadas, ya existen dentro de este ajuste.");

                adjustmentDetail.Warehouse = await WarehouseService.FindAsync(adjustmentDetail.WarehouseId);

                var reference = await ItemReferenceService.FindAsync(adjustmentDetail.ReferenceId);

                adjustmentDetail.ItemReference = reference;

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