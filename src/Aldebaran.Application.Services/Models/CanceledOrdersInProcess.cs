namespace Aldebaran.Application.Services.Models
{
    public class CanceledOrdersInProcess
    {
        public int CustomerOrderInProcessId { get; set; }
        public short CancellationReasonId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime CancellationDate { get; set; }
        public CancellationReason CancellationReason { get; set; } = null!;
        public CustomerOrdersInProcess CustomerOrdersInProcess { get; set; } = null!;
        public Employee Employee { get; set; } = null!;
        public CanceledOrdersInProcess()
        {
            CancellationDate = DateTime.Now;
        }

    }
}
