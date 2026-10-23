using Aldebaran.DataAccess.Core;
using Aldebaran.DataAccess.Core.Atributes;

namespace Aldebaran.DataAccess.Entities
{
    public class NotificationProviderSetting : ITrackeable
    {
        public short NotificationProviderSettingId { get; set; }
        public required string Subject { get; set; }
        [TrackMasked("password", "pwd", "secret", "token")]
        public required string Settings { get; set; }
    }
}
