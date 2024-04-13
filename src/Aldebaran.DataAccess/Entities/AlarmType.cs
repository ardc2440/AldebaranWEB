namespace Aldebaran.DataAccess.Entities
{
    public class AlarmType
    {
        public short AlarmTypeId { get; set; }
        public short DocumentTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }       
        // Reverse navigation
        public ICollection<AlarmMessage> AlarmMessages { get; set; }
        public ICollection<UsersAlarmType> UsersAlarmTypes { get; set; }
        public DocumentType DocumentType { get; set; }
        public AlarmType()
        {            
            AlarmMessages = new List<AlarmMessage>();
            UsersAlarmTypes = new List<UsersAlarmType>();
        }
    }
}
