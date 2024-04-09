using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.WarehouseTransferPages
{
    public partial class AddWarehouseTransferDetail
    {
        #region Injections

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IWarehouseTransferDetailService WarehouseTransferDetailService { get; set; }

        [Inject]
        protected IItemReferenceService ItemReferenceService { get; set; }
        [Inject]
        protected IReferencesWarehouseService ReferencesWarehouseService { get; set; }

        #endregion

        #region Parameters

        [Parameter]
        public ICollection<WarehouseTransferDetail> WarehouseTransferDetails { get; set; }

        [Parameter]
        public WarehouseTransfer warehouseTransfer { get; set; }

        [Parameter]
        public int WarehouseTransferId { get; set; }

        [Parameter]
        public int ReferenceId { get; set; }

        #endregion

        #region Properties

        protected IEnumerable<ItemReference> ItemReferencesForReferenceId { get; set; } = new List<ItemReference>();

        #endregion

        #region Global Variables

        protected bool IsErrorVisible;
        protected string Error = "No se hapodido agregar la referencia";
        protected WarehouseTransferDetail warehouseTransferDetail;
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;
        bool hasWarehouseTransferIdValue;
        bool hasReferenceIdValue;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                ItemReferencesForReferenceId = await ItemReferenceService.GetByStatusAsync(true);

                warehouseTransferDetail.Quantity = 0;
            }
            finally
            {
                isLoadingInProgress = false;
            }

        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            warehouseTransferDetail = new WarehouseTransferDetail();

            hasWarehouseTransferIdValue = parameters.TryGetValue<int>("WarehouseTransferId", out var hasWarehouseTransfer_IDResult);

            if (hasWarehouseTransferIdValue)
            {
                warehouseTransferDetail.WarehouseTransferId = hasWarehouseTransfer_IDResult;
            }

            hasReferenceIdValue = parameters.TryGetValue<int>("ReferenceId", out var hasREFERENCE_IDResult);

            if (hasReferenceIdValue)
            {
                warehouseTransferDetail.ReferenceId = hasREFERENCE_IDResult;
            }

            await base.SetParametersAsync(parameters);
        }

        #endregion

        #region Events

        private async Task<string> ValidateOriginQuantities()
        {
            var msg = String.Empty;

            var referenceWareouse = await ReferencesWarehouseService.GetByReferenceAndWarehouseIdAsync(warehouseTransferDetail.ReferenceId, warehouseTransfer.OriginWarehouseId);

            if (referenceWareouse == null)
                msg = $"La referencia seleccionada no existe en la bodega de origen.";

            if (referenceWareouse.Quantity < warehouseTransferDetail.Quantity)
                msg = $"La cantidad ingresada supera la existencia dentro de la bodega origen.";

            return msg;
        }

        protected async Task FormSubmit()
        {
            try
            {
                IsErrorVisible = false;
                IsSubmitInProgress = true;

                if (WarehouseTransferDetails.Any(ad => ad.ReferenceId == warehouseTransferDetail.ReferenceId))
                    throw new Exception("La referencia seleccionada, ya existe dentro de este traslado.");

                var msg = await ValidateOriginQuantities();

                if (!String.IsNullOrEmpty(msg))
                    throw new Exception(msg);


                DialogService.Close(warehouseTransferDetail);
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

        protected async Task ItemReferenceHandler(ItemReference reference)
        {
            warehouseTransferDetail.ReferenceId = reference?.ReferenceId ?? 0;
            warehouseTransferDetail.ItemReference = warehouseTransferDetail.ReferenceId == 0 ? null : ItemReferencesForReferenceId.Single(s => s.ReferenceId == warehouseTransferDetail.ReferenceId);
        }

        #endregion
    }
}