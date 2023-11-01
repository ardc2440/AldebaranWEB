using Aldebaran.Web.Models.AldebaranDb;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Shared
{
    public partial class InventoryQuantities
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

        [Parameter]
        public ItemReference Reference { get; set; }

        protected ICollection<ReferencesWarehouse> referencesWarehouses;
        protected RadzenDataGrid<ReferencesWarehouse> referencesWarehousesGrid;
        protected ICollection<GroupPurchaseOrderDetail> totalTransitOrders;
        protected RadzenDataGrid<GroupPurchaseOrderDetail> totalTransitOrdersGrid;
        protected int inventoryQuantity = 0;
        protected int reservedQuantity = 0;
        protected int orderedQuantity = 0;

        async Task CleanData()
        {
            Reference = null;
            referencesWarehouses = new List<ReferencesWarehouse>();
            totalTransitOrders = new List<GroupPurchaseOrderDetail>();
        }

        protected override async Task OnInitializedAsync()
        {
            await CleanData();
        }

        public async Task Refresh(ItemReference reference)
        {
            if (reference == null)
                return;

            Reference = reference;

            var referencesWarehouses = await AldebaranDbService.GetReferencesWarehouses(new Query { Filter = $@"i => i.REFERENCE_ID == {Reference.REFERENCE_ID}", Expand = "Warehouse" });

            if (referencesWarehouses != null)
            {
                this.referencesWarehouses = referencesWarehouses.ToList();
            }

            var totalTransitOrders = await AldebaranDbService.GetTotalTransitOrdersPurchaseByReferenceId(Reference.REFERENCE_ID);
            if (referencesWarehouses != null)
            {
                this.totalTransitOrders = totalTransitOrders;
            }

            inventoryQuantity = Reference?.INVENTORY_QUANTITY ?? 0;
            reservedQuantity = Reference?.RESERVED_QUANTITY ?? 0;
            orderedQuantity = Reference?.ORDERED_QUANTITY ?? 0;
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);

            if (Reference == null)
                return;

            await Refresh(Reference);
        }
    }
}