namespace Aldebaran.DataAccess.Entities
{
    public class ModifiedCustomerOrder
    {
        public int ModifiedCustomerOrderId { get; set; }
        public int CustomerOrderId { get; set; }
        public short ModificationReasonId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime ModificationDate { get; set; }
        public CustomerOrder CustomerOrder { get; set; }
        public Employee Employee { get; set; }
        public ModificationReason ModificationReason { get; set; }
        public ModifiedCustomerOrder()
        {
            ModificationDate = DateTime.Now;
        }
    }
}
