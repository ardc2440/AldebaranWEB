using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Models.ViewModels;
using Aldebaran.Web.Shared;
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
        protected IWarehouseService WarehouseService { get; set; }

        [Inject]
        protected IItemReferenceService ItemReferenceService { get; set; }

        #endregion

        #region Parameters

        [Parameter]
        public DetailInProcess DetailInProcess { get; set; }

        [Parameter]
        public short WAREHOUSE_ID { get; set; }

        #endregion

        #region Global Variables

        protected bool errorVisible;
        protected string alertMessage;
        protected bool isSubmitInProgress;
        protected InventoryQuantities QuantitiesPanel;
        protected IEnumerable<Warehouse> warehousesForWAREHOUSEID;
        protected bool hasWAREHOUSE_IDValue;
        protected DetailInProcess detailInProcess;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            warehousesForWAREHOUSEID = await WarehouseService.GetAsync();

            detailInProcess = new DetailInProcess()
            {
                CUSTOMER_ORDER_DETAIL_ID = DetailInProcess.CUSTOMER_ORDER_DETAIL_ID,
                DELIVERED_QUANTITY = DetailInProcess.DELIVERED_QUANTITY,
                BRAND = DetailInProcess.BRAND,
                WAREHOUSE_ID = DetailInProcess.WAREHOUSE_ID,
                THIS_QUANTITY = DetailInProcess.THIS_QUANTITY,
                PENDING_QUANTITY = DetailInProcess.PENDING_QUANTITY,
                PROCESSED_QUANTITY = DetailInProcess.PROCESSED_QUANTITY,
                REFERENCE_DESCRIPTION = DetailInProcess.REFERENCE_DESCRIPTION,
                REFERENCE_ID = DetailInProcess.REFERENCE_ID,
                ItemReference = DetailInProcess.ItemReference
            };

            if (detailInProcess.THIS_QUANTITY == 0)
                detailInProcess.THIS_QUANTITY = detailInProcess.PROCESSED_QUANTITY;
        }

        #endregion

        #region Events

        protected async Task FormSubmit()
        {
            try
            {
                errorVisible = false;
                isSubmitInProgress = true;

                if ((detailInProcess.PROCESSED_QUANTITY + DetailInProcess.THIS_QUANTITY) < detailInProcess.THIS_QUANTITY)
                    throw new Exception("La cantidad de este despacho debe ser menor o igual a la cantidad pendiente del artículo");

                if (await DialogService.Confirm("Está seguro que desea despachar esta cantidad de la referencia?", "Confirmar") == true)
                {
                    DetailInProcess.WAREHOUSE_ID = detailInProcess.WAREHOUSE_ID;
                    DetailInProcess.PROCESSED_QUANTITY = (detailInProcess.PROCESSED_QUANTITY + DetailInProcess.THIS_QUANTITY) - detailInProcess.THIS_QUANTITY;
                    DetailInProcess.THIS_QUANTITY = detailInProcess.THIS_QUANTITY;

                    DialogService.Close(DetailInProcess);
                }
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

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            hasWAREHOUSE_IDValue = parameters.TryGetValue<short>("WAREHOUSE_ID", out var hasWAREHOUSE_IDResult);

            if (hasWAREHOUSE_IDValue)
            {
                detailInProcess.WAREHOUSE_ID = hasWAREHOUSE_IDResult;
            }

            await base.SetParametersAsync(parameters);
        }

        protected async Task ItemReferenceHandler()
        {
            await QuantitiesPanel.Refresh(detailInProcess.REFERENCE_ID);
        }

        #endregion

    }
}