namespace Aldebaran.DataAccess.Entities
{
    public class ClosedCustomerOrder
    {
        public int ClosedCustomerOrderId { get; set; }
        public int CustomerOrderId { get; set; }
        public short CloseCustomerOrderReasonId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime CloseDate { get; set; }
        public CloseCustomerOrderReason CloseCustomerOrderReason { get; set; }
        public CustomerOrder CustomerOrder { get; set; }
        public Employee Employee { get; set; }
        public ClosedCustomerOrder()
        {
            CloseDate = DateTime.Now;
        }
    }
}
