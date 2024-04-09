namespace Aldebaran.Application.Services.Models
{
    public class NotificationTemplate
    {
        public short NotificationTemplateId { get; set; }
        public required string Name { get; set; }
        public required string Subject { get; set; }
        public required string Message { get; set; }
    }
}
