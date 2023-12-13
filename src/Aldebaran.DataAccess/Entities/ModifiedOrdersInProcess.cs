namespace Aldebaran.DataAccess.Entities
{
    public class ModifiedOrdersInProcess
    {
        public int ModifiedCustomerOrderInProcessId { get; set; }
        public int CustomerOrderInProcessId { get; set; }
        public short ModificationReasonId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime ModificationDate { get; set; }
        public CustomerOrdersInProcess CustomerOrdersInProcess { get; set; }
        public Employee Employee { get; set; }
        public ModificationReason ModificationReason { get; set; }
        public ModifiedOrdersInProcess()
        {
            ModificationDate = DateTime.Now;
        }
    }
}
