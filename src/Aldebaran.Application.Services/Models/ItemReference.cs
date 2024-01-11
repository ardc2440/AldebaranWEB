namespace Aldebaran.Application.Services.Models
{
    public class ItemReference
    {
        public int ReferenceId { get; set; }
        public int ItemId { get; set; }
        public string ReferenceCode { get; set; }
        public string ProviderReferenceCode { get; set; }
        public string ReferenceName { get; set; }
        public string ProviderReferenceName { get; set; }
        public string Notes { get; set; }
        public int InventoryQuantity { get; set; }
        public int OrderedQuantity { get; set; }
        public int ReservedQuantity { get; set; }
        public int WorkInProcessQuantity { get; set; }
        public bool IsActive { get; set; }
        public bool IsSoldOut { get; set; }
        public int AlarmMinimumQuantity { get; set; }
        // Reverse navigation
        public ICollection<AdjustmentDetail> AdjustmentDetails { get; set; }
        public ICollection<CustomerOrderDetail> CustomerOrderDetails { get; set; }
        public ICollection<CustomerReservationDetail> CustomerReservationDetails { get; set; }
        public ICollection<ProviderReference> ProviderReferences { get; set; }
        public ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
        public ICollection<ReferencesWarehouse> ReferencesWarehouses { get; set; }
        public ICollection<WarehouseTransferDetail> WarehouseTransferDetails { get; set; }
        public Item Item { get; set; }
        public ItemReference()
        {
            InventoryQuantity = 0;
            OrderedQuantity = 0;
            ReservedQuantity = 0;
            WorkInProcessQuantity = 0;
            IsActive = true;
            IsSoldOut = false;
            AlarmMinimumQuantity = 0;
            AdjustmentDetails = new List<AdjustmentDetail>();
            CustomerOrderDetails = new List<CustomerOrderDetail>();
            CustomerReservationDetails = new List<CustomerReservationDetail>();
            ProviderReferences = new List<ProviderReference>();
            PurchaseOrderDetails = new List<PurchaseOrderDetail>();
            ReferencesWarehouses = new List<ReferencesWarehouse>();
        }
    }
}
