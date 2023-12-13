namespace Aldebaran.DataAccess.Entities
{
    public class ModifiedCustomerReservation
    {
        public int ModifiedCustomerReservationId { get; set; }
        public int CustomerReservationId { get; set; }
        public short ModificationReasonId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime ModificationDate { get; set; }
        public CustomerReservation CustomerReservation { get; set; }
        public Employee Employee { get; set; }
        public ModificationReason ModificationReason { get; set; }
        public ModifiedCustomerReservation()
        {
            ModificationDate = DateTime.Now;
        }
    }
}
