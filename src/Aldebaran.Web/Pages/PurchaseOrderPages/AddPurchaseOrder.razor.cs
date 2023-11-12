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

        protected bool errorVisible;
        protected PurchaseOrder purchaseOrder;
        protected IEnumerable<Provider> providersForPROVIDERID;
        protected IEnumerable<ShipmentForwarderAgentMethod> shipmentForwarderAgentMethods;

        protected ICollection<PurchaseOrderDetail> purchaseOrderDetails;
        protected RadzenDataGrid<PurchaseOrderDetail> purchaseOrderDetailGrid;

        protected ICollection<PurchaseOrderActivity> purchaseOrderActivities;
        protected RadzenDataGrid<PurchaseOrderActivity> purchaseOrderActivityGrid;

        protected override async Task OnInitializedAsync()
        {
            purchaseOrderDetails = new List<PurchaseOrderDetail>();
            purchaseOrderActivities = new List<PurchaseOrderActivity>();

            purchaseOrder = new PurchaseOrder();
            providersForPROVIDERID = await AldebaranDbService.GetProviders(new Query { Expand = "City.Department.Country" });
        }

        protected async Task FormSubmit()
        {
            try
            {
                var now = DateTime.UtcNow;
                // Complementar la orden compra
                purchaseOrder.ORDER_NUMBER = "0000001"; // Count() + 1 <= Ojo que se puede presetnar que dos usuarios creen una orden al mismo tiempo por se le asignaria el mismo numero de orden y seria al momento de guardar que por el indice de la tabla no podrai guardarse la nueva orden, se sugiere que este numero sea calculado al momento de guardar
                purchaseOrder.CREATION_DATE = now;
                purchaseOrder.EMPLOYEE_ID = 0;//<= se debe obtener de la autenticacion
                purchaseOrder.STATUS_DOCUMENT_TYPE_ID = 0;//<= se debe obtener el tipo de doc: Orden con Status: Pendiente

                // Complemento para purchase-order-activity
                foreach (var activity in purchaseOrderActivities)
                {
                    activity.EMPLOYEE_ID = 0;//<= se debe obtener de la autenticacion
                    activity.CREATION_DATE = now;
                }
                // Se podra realizar un solo save con todas a partir de las relaciones????
                purchaseOrder.PurchaseOrderActivities = purchaseOrderActivities;
                purchaseOrder.PurchaseOrderDetails = purchaseOrderDetails;

                await AldebaranDbService.CreatePurchaseOrder(purchaseOrder);
                DialogService.Close(purchaseOrder);
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
        }

        #region PurchaseOrderDetail
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
        #endregion

        #region PurchaseOrderActivity
        protected async Task AddPurchaseOrderActivityButtonClick(MouseEventArgs args)
        {
            var result = await DialogService.OpenAsync<AddPurchaseOrderActivity>("Nueva actividad");
            if (result == null)
                return;
            var detail = (PurchaseOrderActivity)result;
            purchaseOrderActivities.Add(detail);
            await purchaseOrderActivityGrid.Reload();
        }
        protected async Task DeletePurchaseOrderActivityButtonClick(MouseEventArgs args, PurchaseOrderActivity item)
        {
            if (await DialogService.Confirm("Está seguro que desea eliminar esta actividad?") == true)
            {
                purchaseOrderActivities.Remove(item);
                await purchaseOrderActivityGrid.Reload();
            }
        }
        #endregion

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
            NavigationManager.NavigateTo("purchase-orders");
        }
    }
}