namespace Aldebaran.Application.Services.Models
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
        public ModifiedPurchaseOrder()
        {
            ModificationDate = DateTime.Now;
        }
    }
}
