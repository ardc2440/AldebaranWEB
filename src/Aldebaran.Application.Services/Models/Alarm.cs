namespace Aldebaran.Application.Services.Models
{
    public class Alarm
    {
        public int AlarmId { get; set; }
        public short AlarmTypeId { get; set; }
        public DateTime AlarmGenerationDate { get; set; }
        public int DocumentId { get; set; }
        public bool IsActive { get; set; }
        public string AlarmMessage { get; set; }
        // Reverse navigation
        public ICollection<VisualizedAlarm> VisualizedAlarms { get; set; }
        public AlarmType AlarmType { get; set; }
        public Alarm()
        {
            IsActive = true;
            VisualizedAlarms = new List<VisualizedAlarm>();
        }

    }
}
