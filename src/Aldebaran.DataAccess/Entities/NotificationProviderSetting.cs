using Aldebaran.DataAccess.Core;

namespace Aldebaran.DataAccess.Entities
{
    public class NotificationProviderSetting : ITrackeable
    {
        public short NotificationProviderSettingId { get; set; }
        public required string Subject { get; set; }
        public required string Settings { get; set; }
    }
}
