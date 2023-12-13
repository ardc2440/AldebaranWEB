namespace Aldebaran.Application.Services.Models
{
    public class CanceledPurchaseOrder
    {
        public int PurchaseOrderId { get; set; }
        public short CancellationReasonId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime CancellationDate { get; set; }
        public CancellationReason CancellationReason { get; set; }
        public Employee Employee { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; }
        public CanceledPurchaseOrder()
        {
            CancellationDate = DateTime.Now;
        }

    }
}
