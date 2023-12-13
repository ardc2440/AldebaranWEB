namespace Aldebaran.Application.Services.Models
{
    public class StatusDocumentType
    {
        public short StatusDocumentTypeId { get; set; }
        public string StatusDocumentTypeName { get; set; }
        public short DocumentTypeId { get; set; }
        public string Notes { get; set; }
        public bool EditMode { get; set; }
        public short StatusOrder { get; set; }
        // Reverse navigation
        public ICollection<CustomerOrder> CustomerOrders { get; set; }
        public ICollection<CustomerReservation> CustomerReservations { get; set; }
        public ICollection<PurchaseOrder> PurchaseOrders { get; set; }
        public DocumentType DocumentType { get; set; }
        public StatusDocumentType()
        {
            EditMode = true;
            CustomerOrders = new List<CustomerOrder>();
            CustomerReservations = new List<CustomerReservation>();
            PurchaseOrders = new List<PurchaseOrder>();
        }
    }
}
