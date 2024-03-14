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
        private bool Submitted = false;
        protected bool IsSubmitInProgress;
        protected string Error;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {

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

        #endregion

        #region Events

        protected async Task FormSubmit()
        {
            try
            {
                IsErrorVisible = false;
                IsSubmitInProgress = true;

                if ((detailInProcess.PROCESSED_QUANTITY + DetailInProcess.THIS_QUANTITY) < detailInProcess.THIS_QUANTITY)
                    throw new Exception("La cantidad de este despacho debe ser menor o igual a la cantidad en proceso del artículo");

                if (await DialogService.Confirm("Está seguro que desea despachar esta referencia?", "Confirmar") == true)
                {
                    DetailInProcess.PROCESSED_QUANTITY = (detailInProcess.PROCESSED_QUANTITY + DetailInProcess.THIS_QUANTITY) - detailInProcess.THIS_QUANTITY;
                    DetailInProcess.DELIVERED_QUANTITY = (detailInProcess.DELIVERED_QUANTITY - DetailInProcess.THIS_QUANTITY) + detailInProcess.THIS_QUANTITY;
                    DetailInProcess.THIS_QUANTITY = detailInProcess.THIS_QUANTITY;

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

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }

        #endregion

    }
}