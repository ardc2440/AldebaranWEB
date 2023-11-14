using Aldebaran.Web.Models.AldebaranDb;
using Aldebaran.Web.Models.ViewModels;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.CustomerOrderInProcessPages
{
    public partial class SetQuantityInProcess
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }
        [Inject]
        public AldebaranDbService AldebaranDbService { get; set; }

        [Parameter]
        public DetailInProcess pDetailInProcess { get; set; }

        [Parameter]
        public short WAREHOUSE_ID { get; set; }

        protected bool errorVisible;
        protected string alertMessage;
        protected bool isSubmitInProgress;
        protected InventoryQuantities QuantitiesPanel;
        protected IEnumerable<Warehouse> warehousesForWAREHOUSEID;
        bool hasWAREHOUSE_IDValue;
        public DetailInProcess detailInProcess { get; set; }

        protected override async Task OnInitializedAsync()
        {
            warehousesForWAREHOUSEID = await AldebaranDbService.GetWarehouses();

            detailInProcess = new DetailInProcess()
            {
                CUSTOMER_ORDER_DETAIL_ID = pDetailInProcess.CUSTOMER_ORDER_DETAIL_ID,
                DELIVERED_QUANTITY = pDetailInProcess.DELIVERED_QUANTITY,
                BRAND = pDetailInProcess.BRAND,
                WAREHOUSE_ID = pDetailInProcess.WAREHOUSE_ID,
                THIS_QUANTITY = pDetailInProcess.THIS_QUANTITY,
                PENDING_QUANTITY = pDetailInProcess.PENDING_QUANTITY,
                PROCESSED_QUANTITY = pDetailInProcess.PROCESSED_QUANTITY,
                REFERENCE_DESCRIPTION = pDetailInProcess.REFERENCE_DESCRIPTION,
                REFERENCE_ID = pDetailInProcess.REFERENCE_ID
            };

            if (detailInProcess.THIS_QUANTITY == 0)
                detailInProcess.THIS_QUANTITY = detailInProcess.PENDING_QUANTITY;
        }

        protected async Task FormSubmit()
        {
            try
            {
                errorVisible = false;
                isSubmitInProgress = true;

                if ((detailInProcess.PENDING_QUANTITY + pDetailInProcess.THIS_QUANTITY) < detailInProcess.THIS_QUANTITY)
                    throw new Exception("La cantidad de este traslado debe ser menor o igual a la cantidad pendiente del artículo");

                if (await DialogService.Confirm("Está seguro que desea enviar a proceso esta cantidad de la referencia?", "Confirmar") == true)
                {
                    pDetailInProcess.WAREHOUSE_ID = detailInProcess.WAREHOUSE_ID;
                    pDetailInProcess.PENDING_QUANTITY = (detailInProcess.PENDING_QUANTITY + pDetailInProcess.THIS_QUANTITY) - detailInProcess.THIS_QUANTITY;
                    pDetailInProcess.THIS_QUANTITY = detailInProcess.THIS_QUANTITY;

                    DialogService.Close(pDetailInProcess);
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

    }
}