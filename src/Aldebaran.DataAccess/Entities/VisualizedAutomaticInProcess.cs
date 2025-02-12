using Aldebaran.DataAccess.Core;

namespace Aldebaran.DataAccess.Entities
{
    public class VisualizedAutomaticInProcess : ITrackeable
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
