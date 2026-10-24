namespace Aldebaran.DataAccess.Entities
{
    public class StatusDocumentType
    {
        public short StatusDocumentTypeId { get; set; }
        public string StatusDocumentTypeName { get; set; } = null!;
        public short DocumentTypeId { get; set; }
        public string Notes { get; set; } = null!;
        public bool EditMode { get; set; }
        public short StatusOrder { get; set; }
        public DocumentType DocumentType { get; set; } = null!;

        // Reverse navigation
        public ICollection<CustomerOrderShipment> CustomerOrderShipments { get; set; } = null!;
        public ICollection<CustomerOrdersInProcess> CustomerOrdersInProcesses { get; set; } = null!;
        public ICollection<Adjustment> Adjustments { get; set; }
        public ICollection<CustomerOrder> CustomerOrders { get; set; }
        public ICollection<CustomerReservation> CustomerReservations { get; set; }
        public ICollection<PurchaseOrder> PurchaseOrders { get; set; }
        public ICollection<WarehouseTransfer> WarehouseTransfers { get; set; } = null!;
        public ICollection<CancellationRequest> CancellationRequests { get; set; } = null!;

        public StatusDocumentType()
        {
            EditMode = true;
            Adjustments = new List<Adjustment>();
            CustomerOrders = new List<CustomerOrder>();
            CustomerReservations = new List<CustomerReservation>();
            PurchaseOrders = new List<PurchaseOrder>();
        }
    }
}
