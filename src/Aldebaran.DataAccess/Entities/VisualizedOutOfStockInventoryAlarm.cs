using Aldebaran.DataAccess.Core;

namespace Aldebaran.DataAccess.Entities
{
    public class VisualizedOutOfStockInventoryAlarm :ITrackeable
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
