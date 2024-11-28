using Aldebaran.DataAccess.Core;

namespace Aldebaran.DataAccess.Entities
{
    public class CanceledCustomerOrder : ITrackeable
    {
        public int CustomerOrderId { get; set; }
        public short CancellationReasonId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime CancellationDate { get; set; }
        public CancellationReason CancellationReason { get; set; }
        public CustomerOrder CustomerOrder { get; set; }
        public Employee Employee { get; set; }
        public CanceledCustomerOrder()
        {
            CancellationDate = DateTime.Now;
        }
    }
}
