using Aldebaran.Infraestructure.Core.Model;

namespace Aldebaran.Infraestructure.Core.Queue
{
    public class FakeQueue : IQueue
    {
        public Task Dequeue<TModel>(Func<QueueMessage<TModel>, Task> code)
        {
            return Task.FromResult(default(TModel));
        }
        public void Enqueue<TModel>(TModel request, IDictionary<string, object>? metadata = null)
        {
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
