namespace Aldebaran.Application.Services.Models
{
    public class Reason
    {
        public short ReasonId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime Date
        {
            get
            {
                return DateTime.Now;
            }
        }
    }
}
