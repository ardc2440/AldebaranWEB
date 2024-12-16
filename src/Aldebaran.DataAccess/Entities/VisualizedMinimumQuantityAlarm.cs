using Aldebaran.DataAccess.Core;

namespace Aldebaran.DataAccess.Entities
{
    public class VisualizedMinimumQuantityAlarm : ITrackeable
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
