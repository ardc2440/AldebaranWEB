namespace Aldebaran.Application.Services.Models
{
    public class VisualizedLocalWarehouseAlarm
    {
        public int LocalWarehouseAlarmId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime VisualizedDate { get; set; } = DateTime.Now;
        public LocalWarehouseAlarm? LocalWarehouseAlarm { get; set; }
        public Employee? Employee { get; set; }
    }
}
