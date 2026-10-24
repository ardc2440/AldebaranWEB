using Aldebaran.DataAccess.Core;
namespace Aldebaran.DataAccess.Entities
{
    public class ModifiedCustomerReservation : ITrackeable
    {
        public int ModifiedCustomerReservationId { get; set; }
        public int CustomerReservationId { get; set; }
        public short ModificationReasonId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime ModificationDate { get; set; }
        public CustomerReservation CustomerReservation { get; set; } = null!;
        public Employee Employee { get; set; } = null!;
        public ModificationReason ModificationReason { get; set; } = null!;
        public ModifiedCustomerReservation()
        {
            ModificationDate = DateTime.Now;
        }
    }
}
