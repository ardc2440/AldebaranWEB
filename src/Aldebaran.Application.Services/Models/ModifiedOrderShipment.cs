namespace Aldebaran.Application.Services.Models
{
    public class ModifiedOrderShipment
    {
        public int ModifiedCustomerShipmentId { get; set; }
        public int CustomerOrderShipmentId { get; set; }
        public short ModificationReasonId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime ModificationDate { get; set; }
        public CustomerOrderShipment CustomerOrderShipment { get; set; }
        public Employee Employee { get; set; }
        public ModificationReason ModificationReason { get; set; }
        public ModifiedOrderShipment()
        {
            ModificationDate = DateTime.Now;
        }
    }
}
