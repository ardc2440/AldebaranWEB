using Aldebaran.DataAccess.Core;

namespace Aldebaran.DataAccess.Entities
{
    public class CanceledOrderShipment : ITrackeable
    {
        public int CustomerOrderShipmentId { get; set; }
        public short CancellationReasonId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime CancellationDate { get; set; }
        public CancellationReason CancellationReason { get; set; } = null!;
        public CustomerOrderShipment CustomerOrderShipment { get; set; } = null!;
        public Employee Employee { get; set; } = null!;
        public CanceledOrderShipment()
        {
            CancellationDate = DateTime.Now;
        }
    }
}
