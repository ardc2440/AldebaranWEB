namespace Aldebaran.Application.Services.Models
{
    public class Alarm
    {
        public int AlarmId { get; set; }
        public int AlarmMessageId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ExecutionDate { get; set; }
        public int DocumentId { get; set; }
        public bool IsActive { get; set; }
        public AlarmMessage AlarmMessage { get; set; }

        // Reverse navigation
        public ICollection<VisualizedAlarm> VisualizedAlarms { get; set; }
        
        public Alarm()
        {
            IsActive = true;
            VisualizedAlarms = new List<VisualizedAlarm>();
        }

    }
}
