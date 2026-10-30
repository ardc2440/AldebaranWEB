using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Models.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.CustomerOrderShipmentPages
{
    public partial class SetQuantityShipment
    {
        #region Injections

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IItemReferenceService ItemReferenceService { get; set; }

        #endregion

        #region Parameters

        [Parameter]
        public DetailInProcess DetailInProcess { get; set; }

        #endregion

        #region Global Variables

        protected DetailInProcess detailInProcess;
        protected ItemReference ItemReference { get; set; }
        protected bool IsErrorVisible;
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;
        protected string Error;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                detailInProcess = new DetailInProcess()
                {
                    CUSTOMER_ORDER_DETAIL_ID = DetailInProcess.CUSTOMER_ORDER_DETAIL_ID,
                    DELIVERED_QUANTITY = DetailInProcess.DELIVERED_QUANTITY,
                    BRAND = DetailInProcess.BRAND,
                    THIS_QUANTITY = DetailInProcess.THIS_QUANTITY,
                    PENDING_QUANTITY = DetailInProcess.PENDING_QUANTITY,
                    PROCESSED_QUANTITY = DetailInProcess.PROCESSED_QUANTITY,
                    REFERENCE_DESCRIPTION = DetailInProcess.REFERENCE_DESCRIPTION,
                    REFERENCE_ID = DetailInProcess.REFERENCE_ID,
                    ItemReference = DetailInProcess.ItemReference
                };

                if (detailInProcess.THIS_QUANTITY == 0)
                    detailInProcess.THIS_QUANTITY = detailInProcess.PROCESSED_QUANTITY;

                ItemReference = await ItemReferenceService.FindAsync(detailInProcess.REFERENCE_ID);
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
            /* Evita un segundo envío (doble clic / Enter) mientras el primero está en proceso */
            if (IsSubmitInProgress)
                return;

            try
            {
                IsErrorVisible = false;
                IsSubmitInProgress = true;

                /* La cantidad se captura una sola vez: se valida y se aplica el mismo valor,
                   aunque el campo cambie mientras se muestra la confirmación */
                var quantity = detailInProcess.THIS_QUANTITY;

                ValidateQuantity(quantity);

                if (await DialogService.Confirm($"Está seguro que desea despachar {quantity:N0} unidades de esta referencia?", "Confirmar") == true)
                {
                    ApplyQuantity(quantity);

                    DialogService.Close(DetailInProcess);
                }
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

        /// <summary>
        /// Máximo permitido: lo que está en proceso más lo que ya estaba asignado a este despacho.
        /// </summary>
        private int MaximumQuantity => detailInProcess.PROCESSED_QUANTITY + DetailInProcess.THIS_QUANTITY;

        private void ValidateQuantity(int quantity)
        {
            if (quantity > MaximumQuantity)
                throw new Exception($"La cantidad de este despacho ({quantity:N0}) debe ser menor o igual a la cantidad en proceso del artículo ({MaximumQuantity:N0})");
        }

        private void ApplyQuantity(int quantity)
        {
            DetailInProcess.PROCESSED_QUANTITY = MaximumQuantity - quantity;
            DetailInProcess.DELIVERED_QUANTITY = (detailInProcess.DELIVERED_QUANTITY - DetailInProcess.THIS_QUANTITY) + quantity;
            DetailInProcess.THIS_QUANTITY = quantity;
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }

        #endregion

    }
}