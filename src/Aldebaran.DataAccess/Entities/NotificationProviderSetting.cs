namespace Aldebaran.DataAccess.Entities
{
    public class NotificationProviderSetting
    {
        public short NotificationProviderSettingId { get; set; }
        public required string Subject { get; set; }
        public required string Settings { get; set; }
    }
}
