namespace Aldebaran.Application.Services.Models
{
    public class AlarmMessage
    {
        public int AlarmMessageId { get; set; }
        public short AlarmTypeId { get; set; }
        public string Message { get; set; }
        public AlarmType AlarmType { get; set; }

    }
}
