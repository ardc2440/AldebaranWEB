using Aldebaran.Infraestructure.Core.Queue;

namespace Aldebaran.Application.NotificationProcessor.Settings
{
    public class QueueSettings : IQueueSettings
    {
        public string QueueName => "aldebaran_notification-queue";
    }
}
