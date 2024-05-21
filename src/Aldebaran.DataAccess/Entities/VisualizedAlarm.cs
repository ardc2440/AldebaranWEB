using Aldebaran.DataAccess.Core;

namespace Aldebaran.DataAccess.Entities
{
    public class VisualizedAlarm : ITrackeable
    {
        public int AlarmId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime VisualizedDate { get; set; }
        public Alarm Alarm { get; set; }
        public Employee Employee { get; set; }

        public VisualizedAlarm()
        {
            VisualizedDate = System.DateTime.Now;
        }
    }
}
