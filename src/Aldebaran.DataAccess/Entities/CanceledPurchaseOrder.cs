using Aldebaran.DataAccess.Core;

namespace Aldebaran.DataAccess.Entities
{
    public class CanceledPurchaseOrder : ITrackeable
    {
        public int PurchaseOrderId { get; set; }
        public short CancellationReasonId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime CancellationDate { get; set; }
        public CancellationReason CancellationReason { get; set; } = null!;
        public Employee Employee { get; set; } = null!;
        public PurchaseOrder PurchaseOrder { get; set; } = null!;
        public CanceledPurchaseOrder()
        {
            CancellationDate = DateTime.Now;
        }
    }
}
