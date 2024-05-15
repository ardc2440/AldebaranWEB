using Aldebaran.Infraestructure.Core.Queue;

namespace Aldebaran.Application.NotificationProcessor.Settings
{
    public class QueueSettings : IQueueSettings
    {
        public string DefaultQueue => "aldebaran_notification-queue";

        public string NotificationResultQueue => "aldebaran_notification-result:queue";
    }
}
