using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.PurchaseOrderPages
{
    public partial class AddPurchaseOrderActivity
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
        }
        protected bool errorVisible;
        protected Models.AldebaranDb.PurchaseOrderActivity purchaseOrderActivity;

        protected IEnumerable<Models.AldebaranDb.PurchaseOrder> purchaseOrdersForPURCHASEORDERID;

        protected async Task FormSubmit()
        {
            try
            {
                await AldebaranDbService.CreatePurchaseOrderActivity(purchaseOrderActivity);
                DialogService.Close(purchaseOrderActivity);
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

        [Inject]
        protected SecurityService Security { get; set; }
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            purchaseOrderActivity = new Models.AldebaranDb.PurchaseOrderActivity();

            hasPURCHASE_ORDER_IDValue = parameters.TryGetValue<int>("PURCHASE_ORDER_ID", out var hasPURCHASE_ORDER_IDResult);

            if (hasPURCHASE_ORDER_IDValue)
            {
                purchaseOrderActivity.PURCHASE_ORDER_ID = hasPURCHASE_ORDER_IDResult;
            }
            await base.SetParametersAsync(parameters);
        }
    }
}