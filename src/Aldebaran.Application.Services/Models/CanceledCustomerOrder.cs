namespace Aldebaran.Application.Services.Models
{
    public class CanceledCustomerOrder
    {
        public int CustomerOrderId { get; set; }
        public short CancellationReasonId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime CancellationDate { get; set; }
        public CancellationReason CancellationReason { get; set; } = null!;
        public CustomerOrder CustomerOrder { get; set; } = null!;
        public Employee Employee { get; set; } = null!;
        public CanceledCustomerOrder()
        {
            CancellationDate = DateTime.Now;
        }

    }
}
