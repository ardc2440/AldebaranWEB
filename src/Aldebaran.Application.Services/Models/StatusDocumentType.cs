namespace Aldebaran.Application.Services.Models
{
    public class StatusDocumentType
    {
        public short StatusDocumentTypeId { get; set; }
        public string StatusDocumentTypeName { get; set; } = null!;
        public short DocumentTypeId { get; set; }
        public string? Notes { get; set; }
        public bool EditMode { get; set; }
        public short StatusOrder { get; set; }
        // Reverse navigation
        public ICollection<Adjustment> Adjustments { get; set; }
        public ICollection<CustomerOrder> CustomerOrders { get; set; }
        public ICollection<CustomerReservation> CustomerReservations { get; set; }
        public ICollection<PurchaseOrder> PurchaseOrders { get; set; }
        public ICollection<CustomerOrdersInProcess> CustomerOrdersInProcesses { get; set; }
        public ICollection<CustomerOrderShipment> CustomerOrderShipments { get; set; }
        public ICollection<WarehouseTransfer> WarehouseTransfers { get; set; }
        public ICollection<CancellationRequest> CancellationRequests { get; set; }

        public DocumentType DocumentType { get; set; } = null!;
        public StatusDocumentType()
        {
            EditMode = true;
            Adjustments = new List<Adjustment>();
            CustomerOrders = new List<CustomerOrder>();
            CustomerReservations = new List<CustomerReservation>();
            PurchaseOrders = new List<PurchaseOrder>();
            CustomerOrdersInProcesses = new List<CustomerOrdersInProcess>();
            CustomerOrderShipments = new List<CustomerOrderShipment>();
            WarehouseTransfers = new List<WarehouseTransfer>();
        }
    }
}
