namespace Aldebaran.Application.Services.Models
{
    public class AlarmMessage
    {
        public int AlarmMessageId { get; set; }
        public short AlarmTypeId { get; set; }
        public string Message { get; set; } = null!;
        public AlarmType AlarmType { get; set; } = null!;
        public ICollection<Alarm> Alarms { get; set; } 

        public AlarmMessage() 
        {
            Alarms = new List<Alarm>();
        }
    }
}
