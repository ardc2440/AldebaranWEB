using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.PurchaseOrderPages
{
    public partial class AddPurchaseOrderDetail
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

        protected override async Task OnInitializedAsync()
        {

            purchaseOrdersForPURCHASEORDERID = await AldebaranDbService.GetPurchaseOrders();

            itemReferencesForREFERENCEID = await AldebaranDbService.GetItemReferences();

            warehousesForWAREHOUSEID = await AldebaranDbService.GetWarehouses();
        }
        protected bool errorVisible;
        protected Models.AldebaranDb.PurchaseOrderDetail purchaseOrderDetail;

        protected IEnumerable<Models.AldebaranDb.PurchaseOrder> purchaseOrdersForPURCHASEORDERID;

        protected IEnumerable<Models.AldebaranDb.ItemReference> itemReferencesForREFERENCEID;

        protected IEnumerable<Models.AldebaranDb.Warehouse> warehousesForWAREHOUSEID;

        protected async Task FormSubmit()
        {
            try
            {
                await AldebaranDbService.CreatePurchaseOrderDetail(purchaseOrderDetail);
                DialogService.Close(purchaseOrderDetail);
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }

        bool hasPURCHASE_ORDER_IDValue;

        [Parameter]
        public int PURCHASE_ORDER_ID { get; set; }

        bool hasREFERENCE_IDValue;

        [Parameter]
        public int REFERENCE_ID { get; set; }

        bool hasWAREHOUSE_IDValue;

        [Parameter]
        public short WAREHOUSE_ID { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            purchaseOrderDetail = new Models.AldebaranDb.PurchaseOrderDetail();

            hasPURCHASE_ORDER_IDValue = parameters.TryGetValue<int>("PURCHASE_ORDER_ID", out var hasPURCHASE_ORDER_IDResult);

            if (hasPURCHASE_ORDER_IDValue)
            {
                purchaseOrderDetail.PURCHASE_ORDER_ID = hasPURCHASE_ORDER_IDResult;
            }

            hasREFERENCE_IDValue = parameters.TryGetValue<int>("REFERENCE_ID", out var hasREFERENCE_IDResult);

            if (hasREFERENCE_IDValue)
            {
                purchaseOrderDetail.REFERENCE_ID = hasREFERENCE_IDResult;
            }

            hasWAREHOUSE_IDValue = parameters.TryGetValue<short>("WAREHOUSE_ID", out var hasWAREHOUSE_IDResult);

            if (hasWAREHOUSE_IDValue)
            {
                purchaseOrderDetail.WAREHOUSE_ID = hasWAREHOUSE_IDResult;
            }
            await base.SetParametersAsync(parameters);
        }
    }
}