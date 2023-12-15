using Aldebaran.Web.Models.AldebaranDb;
using Aldebaran.Web.Models.ViewModels;
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

        protected IEnumerable<ReferencesWarehouse> referencesWarehouses;
        protected RadzenDataGrid<ReferencesWarehouse> referencesWarehousesGrid;
        protected IEnumerable<GroupPurchaseOrderDetail> totalTransitOrders;
        protected RadzenDataGrid<GroupPurchaseOrderDetail> totalTransitOrdersGrid;
        protected ICollection<ItemReferenceInventory> itemReferenceInventorys;
        protected RadzenDataGrid<ItemReferenceInventory> itemReferenceInventorysGrid;
        protected int inventoryQuantity = 0;
        protected int reservedQuantity = 0;
        protected int orderedQuantity = 0;

        protected override async Task OnInitializedAsync()
        {
            referencesWarehouses = new List<ReferencesWarehouse>();
            totalTransitOrders = new List<GroupPurchaseOrderDetail>();
            itemReferenceInventorys = new List<ItemReferenceInventory>();
        }

        public async Task Refresh(int reference_id)
        {
            var reference = await AldebaranDbService.GetItemReferenceByReferenceId(reference_id);
            await Refresh(reference);
        }

        public async Task Refresh(ItemReference reference)
        {
            if (reference == null)
                return;

            Reference = reference;

            await RefreshInventory(reference);

            referencesWarehouses = await AldebaranDbService.GetReferencesWarehouses(new Query { Filter = $@"i => i.REFERENCE_ID == @0", FilterParameters = new object[] { Reference.REFERENCE_ID }, Expand = "Warehouse" });

            await referencesWarehousesGrid.Reload();

            this.totalTransitOrders = await AldebaranDbService.GetTotalTransitOrdersPurchaseByReferenceId(Reference.REFERENCE_ID);

        }

        public async Task RefreshInventory(ItemReference reference)
        {
            itemReferenceInventorys.Clear();
            itemReferenceInventorys.Add(new ItemReferenceInventory() { Type = "Cantidad", Quantity = Reference?.INVENTORY_QUANTITY ?? 0 });
            itemReferenceInventorys.Add(new ItemReferenceInventory() { Type = "Pedido", Quantity = Reference?.RESERVED_QUANTITY ?? 0 });
            itemReferenceInventorys.Add(new ItemReferenceInventory() { Type = "Reservado", Quantity = Reference?.ORDERED_QUANTITY ?? 0 });

            await itemReferenceInventorysGrid.Reload();
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);

            if (Reference == null)
                return;
        }
    }
}