namespace Aldebaran.DataAccess.Entities
{
    public class AlarmMessage
    {
        public int AlarmMessageId { get; set; }
        public short AlarmTypeId { get; set; }
        public string Message { get; set; }
        public AlarmType AlarmType { get; set; }
        public ICollection<Alarm> Alarms { get; set; }

        public AlarmMessage()
        {
            Alarms = new List<Alarm>();
        }
    }
}
