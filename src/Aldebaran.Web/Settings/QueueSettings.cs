using Aldebaran.Infraestructure.Core.Queue;

namespace Aldebaran.Web.Settings
{
    public class QueueSettings : IQueueSettings
    {
        public string DefaultQueue => "aldebaran_notification-queue";
    }
}
