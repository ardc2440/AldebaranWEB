namespace Aldebaran.Application.Services.Models
{
    public class VisualizedOutOfStockInventoryAlarm
    {
        public int OutOfStockInventoryAlarmId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime VisualizedDate { get; set; }

        public Employee Employee { get; set; }

        public VisualizedOutOfStockInventoryAlarm()
        {
            VisualizedDate = DateTime.Now;
        }
    }
}
