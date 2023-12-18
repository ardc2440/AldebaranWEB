using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Models.ViewModels;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;

namespace Aldebaran.Web.Shared
{
    public partial class InventoryQuantities
    {
        #region Injections
        [Inject]
        public IReferencesWarehouseService ReferencesWarehouseService { get; set; }
        //public AldebaranDbService AldebaranDbService { get; set; }

        [Inject]
        public IItemReferenceService ItemReferenceService { get; set; }

        #endregion

        #region Parameters
        [Parameter]
        public ItemReference Reference { get; set; }
        #endregion

        #region Global Variables
        protected IEnumerable<ReferencesWarehouse> referencesWarehouses;
        protected RadzenDataGrid<ReferencesWarehouse> referencesWarehousesGrid;
        protected IEnumerable<GroupPurchaseOrderDetail> totalTransitOrders;
        protected RadzenDataGrid<GroupPurchaseOrderDetail> totalTransitOrdersGrid;
        protected ICollection<ItemReferenceInventory> itemReferenceInventorys;
        protected RadzenDataGrid<ItemReferenceInventory> itemReferenceInventorysGrid;
        protected int inventoryQuantity = 0;
        protected int reservedQuantity = 0;
        protected int orderedQuantity = 0;

        #endregion

        #region MyRegion
        protected override async Task OnInitializedAsync()
        {
            referencesWarehouses = new List<ReferencesWarehouse>();
            totalTransitOrders = new List<GroupPurchaseOrderDetail>();
            itemReferenceInventorys = new List<ItemReferenceInventory>();
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);

            if (Reference == null)
                return;
        }
        #endregion

        public async Task Refresh(int reference_id)
        {
            var reference = await ItemReferenceService.FindAsync(reference_id);
            await Refresh(reference);
        }

        public async Task Refresh(ItemReference reference)
        {
            if (reference == null)
                return;

            Reference = reference;

            await RefreshInventory(reference);

            referencesWarehouses = await ReferencesWarehouseService.GetByReferenceIdAsync(Reference.ReferenceId);

            await referencesWarehousesGrid.Reload();

            this.totalTransitOrders = await AldebaranDbService.GetTotalTransitOrdersPurchaseByReferenceId(Reference.ReferenceId);

        }

        public async Task RefreshInventory(ItemReference reference)
        {
            itemReferenceInventorys.Clear();
            itemReferenceInventorys.Add(new ItemReferenceInventory() { Type = "Cantidad", Quantity = Reference?.InventoryQuantity ?? 0 });
            itemReferenceInventorys.Add(new ItemReferenceInventory() { Type = "Pedido", Quantity = Reference?.ReservedQuantity ?? 0 });
            itemReferenceInventorys.Add(new ItemReferenceInventory() { Type = "Reservado", Quantity = Reference?.OrderedQuantity ?? 0 });

            await itemReferenceInventorysGrid.Reload();
        }
    }
}