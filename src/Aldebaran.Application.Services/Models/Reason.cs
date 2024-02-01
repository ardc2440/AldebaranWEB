namespace Aldebaran.Application.Services.Models
{
    public class Reason
    {
        public short CancellationReasonId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime Date
        {
            get
            {
                return DateTime.UtcNow;
            }
        }
    }
}
