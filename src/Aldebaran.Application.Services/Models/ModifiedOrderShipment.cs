namespace Aldebaran.Application.Services.Models
{
    public class ModifiedOrderShipment
    {
        public int ModifiedCustomerShipmentId { get; set; }
        public int CustomerOrderShipmentId { get; set; }
        public short ModificationReasonId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime ModificationDate { get; set; }
        public CustomerOrderShipment CustomerOrderShipment { get; set; } = null!;
        public Employee Employee { get; set; } = null!;
        public ModificationReason ModificationReason { get; set; } = null!;
        public ModifiedOrderShipment()
        {
            ModificationDate = DateTime.Now;
        }
    }
}
