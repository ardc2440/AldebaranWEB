namespace Aldebaran.Application.Services.Models
{
    public class CloseCustomerOrderReason
    {
        public short CloseCustomerOrderReasonId { get; set; }
        public string CloseReasonName { get; set; } = null!;
        public string CloseReasonNotes { get; set; } = null!;
        // Reverse navigation
        public ICollection<ClosedCustomerOrder> ClosedCustomerOrders { get; set; }
        public CloseCustomerOrderReason()
        {
            ClosedCustomerOrders = new List<ClosedCustomerOrder>();
        }

    }
}
