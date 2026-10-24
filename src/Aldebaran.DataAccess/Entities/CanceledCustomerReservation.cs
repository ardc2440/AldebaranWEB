using Aldebaran.DataAccess.Core;

namespace Aldebaran.DataAccess.Entities
{
    public class CanceledCustomerReservation : ITrackeable
    {
        public int CustomerReservationId { get; set; }
        public short CancellationReasonId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime CancellationDate { get; set; }
        public CancellationReason CancellationReason { get; set; } = null!;
        public CustomerReservation CustomerReservation { get; set; } = null!;
        public Employee Employee { get; set; } = null!;
        public CanceledCustomerReservation()
        {
            CancellationDate = DateTime.Now;
        }
    }
}
