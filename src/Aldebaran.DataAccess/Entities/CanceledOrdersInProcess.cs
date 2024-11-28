using Aldebaran.DataAccess.Core;

namespace Aldebaran.DataAccess.Entities
{
    public class CanceledOrdersInProcess : ITrackeable
    {
        public int CustomerOrderInProcessId { get; set; }
        public short CancellationReasonId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime CancellationDate { get; set; }
        public CancellationReason CancellationReason { get; set; }
        public CustomerOrdersInProcess CustomerOrdersInProcess { get; set; }
        public Employee Employee { get; set; }
        public CanceledOrdersInProcess()
        {
            CancellationDate = DateTime.Now;
        }
    }
}
