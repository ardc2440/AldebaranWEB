namespace Aldebaran.DataAccess.Entities
{
    public class UsersAlarmType
    {
        public short AlarmTypeId { get; set; }
        public int EmployeeId { get; set; }
        public bool Visualize { get; set; }
        public bool Deactivates { get; set; }
        public AlarmType AlarmType { get; set; }
        public Employee Employee { get; set; }
        public UsersAlarmType()
        {
            Visualize = true;
            Deactivates = false;
        }
    }
}
