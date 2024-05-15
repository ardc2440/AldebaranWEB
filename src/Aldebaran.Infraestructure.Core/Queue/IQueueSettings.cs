namespace Aldebaran.Infraestructure.Core.Queue
{
    public interface IQueueSettings
    {
        string DefaultQueue { get; }
        string NotificationResultQueue { get; }
    }
}
