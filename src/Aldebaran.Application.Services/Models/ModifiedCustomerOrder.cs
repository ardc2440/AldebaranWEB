namespace Aldebaran.Application.Services.Models
{
    public class ModifiedCustomerOrder
    {
        public int ModifiedCustomerOrderId { get; set; }
        public int CustomerOrderId { get; set; }
        public short ModificationReasonId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime ModificationDate { get; set; }
        public CustomerOrder CustomerOrder { get; set; } = null!;
        public Employee Employee { get; set; } = null!;
        public ModificationReason ModificationReason { get; set; } = null!;
        public ModifiedCustomerOrder()
        {
            ModificationDate = DateTime.Now;
        }
    }
}
