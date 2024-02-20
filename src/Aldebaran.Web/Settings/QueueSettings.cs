using Aldebaran.Infraestructure.Core.Queue;

namespace Aldebaran.Web.Settings
{
    public class QueueSettings : IQueueSettings
    {
        public string QueueName => "aldebaran_notification-queue";
    }
}
