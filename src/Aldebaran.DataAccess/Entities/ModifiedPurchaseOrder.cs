namespace Aldebaran.DataAccess.Entities
{
    public class ModifiedPurchaseOrder
    {
        public int ModifiedPurchaseOrderId { get; set; }
        public int PurchaseOrderId { get; set; }
        public short ModificationReasonId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime ModificationDate { get; set; }
        public Employee Employee { get; set; }
        public ModificationReason ModificationReason { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; }
        public ICollection<PurchaseOrderNotification> PurchaseOrderNotifications { get; set; }
        public ModifiedPurchaseOrder()
        {
            ModificationDate = DateTime.Now;
            PurchaseOrderNotifications = new List<PurchaseOrderNotification>();
        }
    }
}
