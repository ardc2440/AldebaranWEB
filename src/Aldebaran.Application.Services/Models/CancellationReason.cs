namespace Aldebaran.Application.Services.Models
{
    public class CancellationReason
    {
        public short CancellationReasonId { get; set; }
        public string CancellationReasonName { get; set; } = null!;
        public short DocumentTypeId { get; set; }
        public string Notes { get; set; } = null!;
        // Reverse navigation
        public ICollection<CanceledCustomerOrder> CanceledCustomerOrders { get; set; }
        public ICollection<CanceledCustomerReservation> CanceledCustomerReservations { get; set; }
        public ICollection<CanceledOrderShipment> CanceledOrderShipments { get; set; }
        public ICollection<CanceledOrdersInProcess> CanceledOrdersInProcesses { get; set; }
        public ICollection<CanceledPurchaseOrder> CanceledPurchaseOrders { get; set; }
        public DocumentType DocumentType { get; set; } = null!;
        public CancellationReason()
        {
            CanceledCustomerOrders = new List<CanceledCustomerOrder>();
            CanceledCustomerReservations = new List<CanceledCustomerReservation>();
            CanceledOrderShipments = new List<CanceledOrderShipment>();
            CanceledOrdersInProcesses = new List<CanceledOrdersInProcess>();
            CanceledPurchaseOrders = new List<CanceledPurchaseOrder>();
        }

    }
}
