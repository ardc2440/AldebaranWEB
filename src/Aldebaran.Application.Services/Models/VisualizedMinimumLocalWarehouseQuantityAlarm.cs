namespace Aldebaran.Application.Services.Models
{    
    public class VisualizedMinimumLocalWarehouseQuantityAlarm 
    {
        public int AlarmId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime VisualizedDate { get; set; }

        public Employee Employee { get; set; }

        public VisualizedMinimumLocalWarehouseQuantityAlarm()
        {
            VisualizedDate = DateTime.Now;
        }
    }
}
