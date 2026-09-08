namespace Aldebaran.Application.Services.Notifications.Models
{
    public class NotificationProcessingResult
    {
        public int NotificationDefinitionId { get; set; }
        public string NotificationName { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public int RecordsFound { get; set; }
        public bool NotificationPublished { get; set; }
    }
}