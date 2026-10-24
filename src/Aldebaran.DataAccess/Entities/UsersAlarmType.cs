using Aldebaran.DataAccess.Core;
namespace Aldebaran.DataAccess.Entities
{
    public class UsersAlarmType : ITrackeable
    {
        public short AlarmTypeId { get; set; }
        public int EmployeeId { get; set; }
        public bool Visualize { get; set; }
        public bool Deactivates { get; set; }
        public AlarmType AlarmType { get; set; } = null!;
        public Employee Employee { get; set; } = null!;
        public UsersAlarmType()
        {
            Visualize = true;
            Deactivates = false;
        }
    }
}
