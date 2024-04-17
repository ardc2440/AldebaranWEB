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

        protected int Available { get; set; } = 0;
        protected int WarehouseAvailable { get; set; } = 0;
        protected int TransitAvailable { get; set; } = 0;
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

        async Task RefreshInventory()
        {
            ItemReferenceInventories.Clear();
            ItemReferenceInventories.Add(new ItemReferenceInventory() { Type = "Cantidad", Quantity = Reference?.InventoryQuantity ?? 0 });
            ItemReferenceInventories.Add(new ItemReferenceInventory() { Type = "Pedido", Quantity = Reference?.OrderedQuantity ?? 0 });
            ItemReferenceInventories.Add(new ItemReferenceInventory() { Type = "Reservado", Quantity = Reference?.ReservedQuantity ?? 0 });
            ItemReferenceInventories.Add(new ItemReferenceInventory() { Type = "En Proceso", Quantity = Reference?.WorkInProcessQuantity ?? 0 });
            if (ItemReferenceInventoryGrid != null)
                await ItemReferenceInventoryGrid.Reload();
            Available = Reference?.InventoryQuantity ?? 0 - Reference?.OrderedQuantity ?? 0 - Reference?.ReservedQuantity ?? 0;
        }
        async Task RefreshWarehouses()
        {
            ReferencesWarehouses = await ReferencesWarehouseService.GetByReferenceIdAsync(Reference.ReferenceId);
            if (ReferencesWarehouseGrid != null)
                await ReferencesWarehouseGrid.Reload();
            WarehouseAvailable = ReferencesWarehouses.Sum(s => s.Quantity);
        }
        async Task RefreshTransitOrders()
        {
            var documentType = await DocumentTypeService.FindByCodeAsync("O");
            var statusOrder = await StatusDocumentTypeService.FindByDocumentAndOrderAsync(documentType.DocumentTypeId, 1);
            var detailInTransit = await PurchaseOrderDetailService.GetTransitDetailOrdersAsync(statusOrder.StatusDocumentTypeId, Reference.ReferenceId);
            GroupPurchaseOrderDetails = detailInTransit.GroupBy(group => group.PurchaseOrder.ExpectedReceiptDate).Select(c => new GroupPurchaseOrderDetail() { ExpectedReceiptDate = c.Key, Quantity = c.Sum(p => p.RequestedQuantity) });
            if (GroupPurchaseOrderDetailGrid != null)
                await GroupPurchaseOrderDetailGrid.Reload();
            TransitAvailable = GroupPurchaseOrderDetails.Sum(s => s.Quantity);
        }
        #endregion
    }
}