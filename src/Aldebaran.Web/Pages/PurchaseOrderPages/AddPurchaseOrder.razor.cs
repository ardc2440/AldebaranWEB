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
        private bool submitted = false;
        protected bool isSubmitInProgress;

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
                isSubmitInProgress = true;
                submitted = true;
                if (!purchaseOrderDetails.Any())
                    return;
                var now = DateTime.UtcNow;
                // Complementar la orden compra
                var employee = (await AldebaranDbService.GetEmployees(new Query { Filter = "i=>i.LOGIN_USER_ID==@0", FilterParameters = new object[] { Security.User.Id } })).Single();
                var documentType = await AldebaranDbService.GetDocumentTypeByCode("O");
                var statusDocumentType = await AldebaranDbService.GetStatusDocumentTypeByDocumentAndOrder(documentType, 1);
                purchaseOrder.CREATION_DATE = now;
                purchaseOrder.EMPLOYEE_ID = employee.EMPLOYEE_ID;
                purchaseOrder.STATUS_DOCUMENT_TYPE_ID = statusDocumentType.STATUS_DOCUMENT_TYPE_ID;
                purchaseOrder.PurchaseOrderActivities = purchaseOrderActivities.Select(s => new PurchaseOrderActivity
                {
                    EXECUTION_DATE = s.EXECUTION_DATE,
                    ACTIVITY_DESCRIPTION = s.ACTIVITY_DESCRIPTION,
                    CREATION_DATE = now,
                    EMPLOYEE_ID = employee.EMPLOYEE_ID,
                    ACTIVITY_EMPLOYEE_ID = s.ACTIVITY_EMPLOYEE_ID
                }).ToList();
                purchaseOrder.PurchaseOrderDetails = purchaseOrderDetails.Select(s => new PurchaseOrderDetail
                {
                    REFERENCE_ID = s.REFERENCE_ID,
                    WAREHOUSE_ID = s.WAREHOUSE_ID,
                    REQUESTED_QUANTITY = s.REQUESTED_QUANTITY,
                }).ToList();

                await AldebaranDbService.CreatePurchaseOrder(purchaseOrder);
                NavigationManager.NavigateTo("purchase-orders");
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
            finally
            {
                isSubmitInProgress = false;
            }
        }

        #region PurchaseOrderDetail
        protected async Task AddPurchaseOrderDetailButtonClick(MouseEventArgs args)
        {
            var providerReferences = await AldebaranDbService.GetProviderReferences(new Query { Filter = $"@i => i.PROVIDER_ID == @0", FilterParameters = new object[] { PROVIDER_ID } });
            var providerReferencesIds = new List<int>();
            foreach (var reference in providerReferences)
            {
                if (!purchaseOrderDetails.Any(a => a.REFERENCE_ID == reference.REFERENCE_ID))
                    providerReferencesIds.Add(reference.REFERENCE_ID);
            }
            //Solo las referencias del proveedor
            var itemReferences = await AldebaranDbService.GetItemReferences(new Query { Filter = "i => @0.Contains(i.REFERENCE_ID)", FilterParameters = new object[] { providerReferencesIds }, Expand = "Item.Line" });
            var result = await DialogService.OpenAsync<AddPurchaseOrderDetail>("Nueva referencia", new Dictionary<string, object> { { "ProviderItemReferences", itemReferences.ToList() } });
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

        protected async Task PROVIDER_IDChange(object providerId)
        {
            PROVIDER_ID = (int)providerId == 0 ? null : (int)providerId;
        }
        private int? PROVIDER_ID { get; set; }
    }
}