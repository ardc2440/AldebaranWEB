namespace Aldebaran.Application.Services.Models
{
    public class VisualizedAlarm
    {
        public int AlarmId { get; set; }
        public int EmployeeId { get; set; }
        public Alarm Alarm { get; set; }
        public Employee Employee { get; set; }
    }
}
