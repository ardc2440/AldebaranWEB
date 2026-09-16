namespace Aldebaran.Application.Services.InventoryMinimumAlerts.Models
{
    public class InventoryMinimumAlertSettings
    {
        public bool Enabled { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public required string NotificationSettings { get; set; }
        public required string NotificationSubject { get; set; } 
        public List<DayOfWeek> DaysOfWeek { get; set; } = new();
        public List<TimeSpan> ExecutionHours { get; set; } = new();
    }
}
