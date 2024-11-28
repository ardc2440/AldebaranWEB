using Aldebaran.DataAccess.Core;

namespace Aldebaran.DataAccess.Entities
{
    public class CanceledPurchaseOrder : ITrackeable
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
