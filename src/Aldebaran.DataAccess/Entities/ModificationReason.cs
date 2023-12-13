namespace Aldebaran.DataAccess.Entities
{
    public class ModificationReason
    {
        public short ModificationReasonId { get; set; }
        public string ModificationReasonName { get; set; }
        public short DocumentTypeId { get; set; }
        public string Notes { get; set; }
        // Reverse navigation
        public ICollection<ModifiedCustomerOrder> ModifiedCustomerOrders { get; set; }
        public ICollection<ModifiedCustomerReservation> ModifiedCustomerReservations { get; set; }
        public ICollection<ModifiedOrderShipment> ModifiedOrderShipments { get; set; }
        public ICollection<ModifiedOrdersInProcess> ModifiedOrdersInProcesses { get; set; }
        public ICollection<ModifiedPurchaseOrder> ModifiedPurchaseOrders { get; set; }
        public DocumentType DocumentType { get; set; }
        public ModificationReason()
        {
            ModifiedCustomerOrders = new List<ModifiedCustomerOrder>();
            ModifiedCustomerReservations = new List<ModifiedCustomerReservation>();
            ModifiedOrderShipments = new List<ModifiedOrderShipment>();
            ModifiedOrdersInProcesses = new List<ModifiedOrdersInProcess>();
            ModifiedPurchaseOrders = new List<ModifiedPurchaseOrder>();
        }
    }
}
