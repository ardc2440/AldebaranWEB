namespace Aldebaran.DataAccess.Entities
{
    public class AlarmType
    {
        public short AlarmTypeId { get; set; }
        public short DocumentTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsManualMessage { get; set; }
        public string TableName { get; set; }
        public string FieldName { get; set; }
        // Reverse navigation
        public ICollection<AlarmMessage> AlarmMessages { get; set; }
        public ICollection<UsersAlarmType> UsersAlarmTypes { get; set; }
        public DocumentType DocumentType { get; set; }
        public AlarmType()
        {
            IsManualMessage = false;
            AlarmMessages = new List<AlarmMessage>();
            UsersAlarmTypes = new List<UsersAlarmType>();
        }
    }
}
