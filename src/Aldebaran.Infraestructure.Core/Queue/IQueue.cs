using Aldebaran.Infraestructure.Core.Model;
namespace Aldebaran.Infraestructure.Core.Queue
{
    /// <summary>
    /// Contrato que permite la gestion de colas
    /// </summary>
    public interface IQueue : IDisposable
    {
        /// <summary>
        /// Permite encolar un mensaje en una cola
        /// </summary>
        /// <typeparam name="TModel">Tipo de mensaje a encolar</typeparam>
        /// <param name="request">Mensaje a encolar</param>
        /// <param name="metadata">Metadata del mensaje</param>
        void Enqueue<TModel>(TModel request, IDictionary<string, object>? metadata = null);
        /// <summary>
        /// Permite desecolar un mensaje de una cola y al mensaje desencolado ser procesado por una funcion
        /// </summary>
        /// <typeparam name="TModel">Tipo de mensaje a encolar</typeparam>
        /// <param name="code">Funcion que procesara el mensaje</param>
        /// <returns></returns>
        Task Dequeue<TModel>(Func<QueueMessage<TModel>, Task> code);
    }
}
