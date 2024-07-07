using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Models.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.CustomerOrderInProcessPages
{
    public partial class SetQuantityInProcess
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

        protected IEnumerable<Warehouse> warehousesForWAREHOUSEID;
        protected bool hasWAREHOUSE_IDValue;
        protected DetailInProcess detailInProcess;
        protected ItemReference ItemReference { get; set; }
        protected bool IsErrorVisible;
        private bool Submitted = false;
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
                warehousesForWAREHOUSEID = await WarehouseService.GetAsync();

                detailInProcess = new DetailInProcess()
                {
                    CUSTOMER_ORDER_DETAIL_ID = DetailInProcess.CUSTOMER_ORDER_DETAIL_ID,
                    DELIVERED_QUANTITY = DetailInProcess.DELIVERED_QUANTITY,
                    BRAND = DetailInProcess.BRAND,
                    WAREHOUSE_ID = (await WarehouseService.FindByCodeAsync(1)).WarehouseId,
                    THIS_QUANTITY = DetailInProcess.THIS_QUANTITY,
                    PENDING_QUANTITY = DetailInProcess.PENDING_QUANTITY,
                    PROCESSED_QUANTITY = DetailInProcess.PROCESSED_QUANTITY,
                    REFERENCE_DESCRIPTION = DetailInProcess.REFERENCE_DESCRIPTION,
                    REFERENCE_ID = DetailInProcess.REFERENCE_ID,
                    ItemReference = DetailInProcess.ItemReference
                };

                if (detailInProcess.THIS_QUANTITY == 0)
                {
                    detailInProcess.THIS_QUANTITY = detailInProcess.PENDING_QUANTITY;
                }
                else
                {
                    if (detailInProcess.THIS_QUANTITY > detailInProcess.PROCESSED_QUANTITY)
                        detailInProcess.THIS_QUANTITY = detailInProcess.PROCESSED_QUANTITY;
                }

                ItemReference = await ItemReferenceService.FindAsync(detailInProcess.REFERENCE_ID);
            }
            finally
            {
                isLoadingInProgress = false;
            }

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
        #endregion

        #region Events

        protected async Task FormSubmit()
        {
            try
            {
                IsErrorVisible = false;
                IsSubmitInProgress = true;

                if ((detailInProcess.PENDING_QUANTITY + DetailInProcess.THIS_QUANTITY) < detailInProcess.THIS_QUANTITY)
                    throw new Exception("La cantidad de este traslado debe ser menor o igual a la cantidad pendiente del artículo");

                if (await DialogService.Confirm("Está seguro que desea enviar a proceso esta referencia?", "Confirmar") == true)
                {
                    DetailInProcess.WAREHOUSE_ID = detailInProcess.WAREHOUSE_ID;
                    DetailInProcess.PENDING_QUANTITY = (detailInProcess.PENDING_QUANTITY + DetailInProcess.THIS_QUANTITY) - detailInProcess.THIS_QUANTITY;
                    DetailInProcess.PROCESSED_QUANTITY = (detailInProcess.PROCESSED_QUANTITY - DetailInProcess.THIS_QUANTITY) + detailInProcess.THIS_QUANTITY;
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