namespace Aldebaran.Application.Services.Models
{
    public class InventoryMinimumAlertSettings
    {
        public bool Enabled { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string NotificationSubject { get; set; } = "NotificationSettings";
        public List<DayOfWeek> DaysOfWeek { get; set; } = new();
        public List<TimeSpan> ExecutionHours { get; set; } = new();
    }
}
