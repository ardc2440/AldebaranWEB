namespace Aldebaran.Application.Services.Models
{
    public class ModifiedOrdersInProcess
    {
        public int ModifiedCustomerOrderInProcessId { get; set; }
        public int CustomerOrderInProcessId { get; set; }
        public short ModificationReasonId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime ModificationDate { get; set; }
        public CustomerOrdersInProcess CustomerOrdersInProcess { get; set; } = null!;
        public Employee Employee { get; set; } = null!;
        public ModificationReason ModificationReason { get; set; } = null!;
        public ModifiedOrdersInProcess()
        {
            ModificationDate = DateTime.Now;
        }
    }
}
