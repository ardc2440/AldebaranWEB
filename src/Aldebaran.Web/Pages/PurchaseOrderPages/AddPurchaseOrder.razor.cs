using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.PurchaseOrderPages
{
    public partial class AddPurchaseOrder
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
            purchaseOrder = new Models.AldebaranDb.PurchaseOrder();

            forwarderAgentsForFORWARDERAGENTID = await AldebaranDbService.GetForwarderAgents();

            providersForPROVIDERID = await AldebaranDbService.GetProviders();

            shipmentForwarderAgentMethodsForSHIPMENTFORWARDERAGENTMETHODID = await AldebaranDbService.GetShipmentForwarderAgentMethods();
        }
        protected bool errorVisible;
        protected Models.AldebaranDb.PurchaseOrder purchaseOrder;

        protected IEnumerable<Models.AldebaranDb.ForwarderAgent> forwarderAgentsForFORWARDERAGENTID;

        protected IEnumerable<Models.AldebaranDb.Provider> providersForPROVIDERID;

        protected IEnumerable<Models.AldebaranDb.ShipmentForwarderAgentMethod> shipmentForwarderAgentMethodsForSHIPMENTFORWARDERAGENTMETHODID;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task FormSubmit()
        {
            try
            {
                await AldebaranDbService.CreatePurchaseOrder(purchaseOrder);
                DialogService.Close(purchaseOrder);
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
    }
}