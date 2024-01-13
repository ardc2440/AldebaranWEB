using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Models.ViewModels;
using Aldebaran.Web.Resources.LocalizedControls;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;

namespace Aldebaran.Web.Shared
{
    public partial class InventoryQuantities
    {
        #region Injections
        [Inject]
        public IReferencesWarehouseService ReferencesWarehouseService { get; set; }

        [Inject]
        public IItemReferenceService ItemReferenceService { get; set; }

        [Inject]
        public IPurchaseOrderDetailService PurchaseOrderDetailService { get; set; }

        [Inject]
        public IDocumentTypeService DocumentTypeService { get; set; }

        [Inject]
        public IStatusDocumentTypeService StatusDocumentTypeService { get; set; }

        #endregion

        #region Parameters
        [Parameter]
        public ItemReference Reference { get; set; }
        #endregion

        #region Global Variables
        protected IEnumerable<ReferencesWarehouse> ReferencesWarehouses = new List<ReferencesWarehouse>();
        protected LocalizedDataGrid<ReferencesWarehouse> ReferencesWarehouseGrid;
        protected IEnumerable<GroupPurchaseOrderDetail> GroupPurchaseOrderDetails = new List<GroupPurchaseOrderDetail>();
        protected LocalizedDataGrid<GroupPurchaseOrderDetail> GroupPurchaseOrderDetailGrid;
        protected List<ItemReferenceInventory> ItemReferenceInventories = new List<ItemReferenceInventory>();
        protected LocalizedDataGrid<ItemReferenceInventory> ItemReferenceInventoryGrid;
        protected int inventoryQuantity = 0;
        protected int reservedQuantity = 0;
        protected int orderedQuantity = 0;

        #endregion

        #region Overrides
        ItemReference ItemReferenceControl { get; set; }
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            if (EqualityComparer<ItemReference>.Default.Equals(Reference, ItemReferenceControl))
                return;

            if (Reference == null)
            {
                await Reset();
                return;
            }
            await RefreshInventory();
            await RefreshWarehouses();
            await RefreshTransitOrders();
            ItemReferenceControl = Reference;
            StateHasChanged();
        }

        async Task Reset()
        {
            ReferencesWarehouses = new List<ReferencesWarehouse>();
            GroupPurchaseOrderDetails = new List<GroupPurchaseOrderDetail>();
            ItemReferenceInventories = new List<ItemReferenceInventory>();
            await ReferencesWarehouseGrid.Reload();
            await GroupPurchaseOrderDetailGrid.Reload();
            await ItemReferenceInventoryGrid.Reload();
        }
        #endregion

        #region Events
        [Obsolete]
        public async Task Refresh(int reference_id)
        {
            //var reference = await ItemReferenceService.FindAsync(reference_id);
            //await Refresh(reference);
        }
        [Obsolete]
        public async Task Refresh(ItemReference reference)
        {
            //if (reference == null)
            //    return;

            //Reference = reference;

            //await RefreshInventory();
            //await RefreshWarehouses();
            //await RefreshTransitOrders();
        }

        async Task RefreshInventory()
        {
            ItemReferenceInventories.Clear();
            ItemReferenceInventories.Add(new ItemReferenceInventory() { Type = "Cantidad", Quantity = Reference?.InventoryQuantity ?? 0 });
            ItemReferenceInventories.Add(new ItemReferenceInventory() { Type = "Pedido", Quantity = Reference?.OrderedQuantity ?? 0 });
            ItemReferenceInventories.Add(new ItemReferenceInventory() { Type = "Reservado", Quantity = Reference?.ReservedQuantity ?? 0 });
            ItemReferenceInventories.Add(new ItemReferenceInventory() { Type = "En Proceso", Quantity = Reference?.WorkInProcessQuantity ?? 0 });
            await ItemReferenceInventoryGrid.Reload();
        }
        async Task RefreshWarehouses()
        {
            ReferencesWarehouses = await ReferencesWarehouseService.GetByReferenceIdAsync(Reference.ReferenceId);
            await ReferencesWarehouseGrid.Reload();
        }
        async Task RefreshTransitOrders()
        {
            var documentType = await DocumentTypeService.FindByCodeAsync("O");
            var statusOrder = await StatusDocumentTypeService.FindByDocumentAndOrderAsync(documentType.DocumentTypeId, 1);
            var detailInTransit = await PurchaseOrderDetailService.GetTransitDetailOrdersAsync(Reference.ReferenceId, statusOrder.StatusDocumentTypeId);
            GroupPurchaseOrderDetails = detailInTransit.GroupBy(group => group.PurchaseOrder.RequestDate).Select(c => new GroupPurchaseOrderDetail() { Request_Date = c.Key, Quantity = c.Sum(p => p.RequestedQuantity) });
            await GroupPurchaseOrderDetailGrid.Reload();
        }
        #endregion
    }
}