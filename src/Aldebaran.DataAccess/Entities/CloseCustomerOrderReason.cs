namespace Aldebaran.DataAccess.Entities
{
    public class CloseCustomerOrderReason
    {
        public short CloseCustomerOrderReasonId { get; set; }
        public string CloseReasonName { get; set; }
        public string CloseReasonNotes { get; set; }
        // Reverse navigation
        public ICollection<ClosedCustomerOrder> ClosedCustomerOrders { get; set; }
        public CloseCustomerOrderReason()
        {
            ClosedCustomerOrders = new List<ClosedCustomerOrder>();
        }
    }
}
