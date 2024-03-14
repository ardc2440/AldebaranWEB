namespace Aldebaran.Application.Services.Models
{
    public class Warehouse
    {
        public short WarehouseId { get; set; }
        public string WarehouseName { get; set; } = null;
        public short WarehouseCode { get; set; }
        // Reverse navigation
        public ICollection<AdjustmentDetail> AdjustmentDetails { get; set; }
        public ICollection<CustomerOrderInProcessDetail> CustomerOrderInProcessDetails { get; set; }
        public ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
        public ICollection<ReferencesWarehouse> ReferencesWarehouses { get; set; }
        public ICollection<WarehouseTransfer> OriginWarehouseTransfers { get; set; }
        public ICollection<WarehouseTransfer> DestinationWarehouseTransfers { get; set; }
        public Warehouse()
        {
            AdjustmentDetails = new List<AdjustmentDetail>();
            CustomerOrderInProcessDetails = new List<CustomerOrderInProcessDetail>();
            PurchaseOrderDetails = new List<PurchaseOrderDetail>();
            ReferencesWarehouses = new List<ReferencesWarehouse>();
            OriginWarehouseTransfers = new List<WarehouseTransfer>();
            DestinationWarehouseTransfers = new List<WarehouseTransfer>();
        }
    }
}
