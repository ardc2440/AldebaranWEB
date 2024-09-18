namespace Aldebaran.Application.Services.Models
{    
    public class VisualizedMinimumQuantityAlarm
    {
        public int MinimumQuantityAlarmId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime VisualizedDate { get; set; }

        public Employee Employee { get; set; }

        public VisualizedMinimumQuantityAlarm()
        {
            VisualizedDate = DateTime.Now;
        }
    }
}
