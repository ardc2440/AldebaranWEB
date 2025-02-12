namespace Aldebaran.Application.Services.Models
{    
    public class VisualizedAutomaticInProcess
    {
        public int AlarmId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime VisualizedDate { get; set; }

        public Employee Employee { get; set; }

        public VisualizedAutomaticInProcess()
        {
            VisualizedDate = DateTime.Now;
        }
    }
}
