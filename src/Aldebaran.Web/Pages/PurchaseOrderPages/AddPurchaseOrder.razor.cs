using Aldebaran.Web.Models.AldebaranDb;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

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
        protected SecurityService Security { get; set; }

        [Inject]
        public AldebaranDbService AldebaranDbService { get; set; }

        protected DateTime Now { get; set; }
        protected bool errorVisible;
        protected PurchaseOrder purchaseOrder;
        protected IEnumerable<Provider> providersForPROVIDERID;
        protected IEnumerable<ShipmentForwarderAgentMethod> shipmentForwarderAgentMethods;
        protected ICollection<PurchaseOrderDetail> purchaseOrderDetails;
        protected RadzenDataGrid<PurchaseOrderDetail> purchaseOrderDetailGrid;

        protected IEnumerable<PurchaseOrderActivity> purchaseOrderActivities;
        protected RadzenDataGrid<PurchaseOrderActivity> purchaseOrderActivityGrid;

        protected override async Task OnInitializedAsync()
        {
            Now = DateTime.UtcNow.AddDays(-1);
            purchaseOrderDetails = new List<PurchaseOrderDetail>();
            purchaseOrderActivities = new List<PurchaseOrderActivity>();

            purchaseOrder = new PurchaseOrder();
            providersForPROVIDERID = await AldebaranDbService.GetProviders(new Query { Expand = "City.Department.Country" });
        }

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

        protected async Task AddPurchaseOrderDetailButtonClick(MouseEventArgs args)
        {
            var result = await DialogService.OpenAsync<AddPurchaseOrderDetail>("Nueva referencia");
            if (result == null)
                return;
            var detail = (PurchaseOrderDetail)result;
            purchaseOrderDetails.Add(detail);
            await purchaseOrderDetailGrid.Reload();
        }

        protected async Task DeletePurchaseOrderDetailButtonClick(MouseEventArgs args, PurchaseOrderDetail item)
        {
            if (await DialogService.Confirm("Está seguro que desea eliminar esta referencia?") == true)
            {
                purchaseOrderDetails.Remove(item);
                await purchaseOrderDetailGrid.Reload();
            }
        }

        protected async Task AgentForwarderHandler(ForwarderAgent agent)
        {
            purchaseOrder.FORWARDER_AGENT_ID = agent?.FORWARDER_AGENT_ID ?? 0;
            if (purchaseOrder.FORWARDER_AGENT_ID == 0)
            {
                shipmentForwarderAgentMethods = new List<ShipmentForwarderAgentMethod>();
                return;
            }
            shipmentForwarderAgentMethods = await AldebaranDbService.GetShipmentForwarderAgentMethods(new Query { Filter = "i=>i.FORWARDER_AGENT_ID=@0", FilterParameters = new object[] { purchaseOrder.FORWARDER_AGENT_ID }, Expand = "ShipmentMethod" }); ;
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}