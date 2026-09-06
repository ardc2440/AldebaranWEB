namespace Aldebaran.Web.Models
{
    public class NotificationEvent
    {
        public int NotificationDefinitionId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string NotificationMessage { get; set; }

        public IEnumerable<string> Roles { get; set; }
    }
}
