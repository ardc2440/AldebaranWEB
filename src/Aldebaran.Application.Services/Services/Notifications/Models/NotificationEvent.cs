namespace Aldebaran.Application.Services.Notifications.Models
{
    public class NotificationEvent
    {
        public int NotificationDefinitionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string NotificationMessage { get; set; } = string.Empty;
        public string? QueryParameters { get; set; }
        public IEnumerable<string> Roles { get; set; } = Enumerable.Empty<string>();
    }
}