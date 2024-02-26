namespace Aldebaran.Infraestructure.Core.Model
{
    /// <summary>
    /// Modelo de orquestador de mesnaes
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueueMessage<T>
    {
        public required T Message { get; set; }

        public required IDictionary<string, string> Metadata { get; set; }
    }
}
