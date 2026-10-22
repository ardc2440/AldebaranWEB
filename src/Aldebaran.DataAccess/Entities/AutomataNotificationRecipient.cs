using Aldebaran.DataAccess.Core;
namespace Aldebaran.DataAccess.Entities
{
    public class AutomataNotificationRecipient : ITrackeable
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string NotificationType { get; set; } = null!;
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? Notes { get; set; }
    }
}
