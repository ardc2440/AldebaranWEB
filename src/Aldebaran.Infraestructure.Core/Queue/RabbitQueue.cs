using Aldebaran.Infraestructure.Core.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Aldebaran.Infraestructure.Core.Queue
{
    /// <summary>
    /// Implementacion haciendo uso de RabbitMQ para el manejo de colas
    /// </summary>
    public class RabbitQueue : IQueue
    {
        /// <summary>
        /// Dependencia de Conexion de RabbitMQ
        /// </summary>
        private readonly IConnection Connection;
        /// <summary>
        /// Channel de consumo de mensajes
        /// </summary>
        private IModel? channel;
        /// <summary>
        /// Dependencia de Logger
        /// </summary>
        private readonly ILogger Logger;

        /// <summary>
        /// Identificador del consumidor de mensajes
        /// </summary>
        private string? ConsumerTag;

        private readonly string QueueName;
        private const int MaxNotificationAttempts = 5;
        public RabbitQueue(IConnection connection, ILogger<RabbitQueue> logger, IQueueSettings queueSettings)
        {
            Connection = connection ?? throw new ArgumentNullException(nameof(IConnection));
            Logger = logger ?? throw new ArgumentNullException(nameof(ILogger));
            QueueName = queueSettings?.QueueName ?? throw new ArgumentNullException($"{nameof(QueueName)}");
        }
        /// <summary>
        /// Permite encolar un mensaje en una cola
        /// </summary>
        /// <typeparam name="TModel">Tipo de mensaje a encolar</typeparam>
        /// <param name="queue">Nombre de la cola a almacenar el mensaje</param>
        /// <param name="request">Mensaje a encolar</param>
        /// <param name="metadata">Metadata del mensaje</param>
        public void Enqueue<TModel>(TModel request, IDictionary<string, object>? metadata = null)
        {
            using var channel = Connection.CreateModel();
            channel.QueueDeclare(queue: QueueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var bproperties = channel.CreateBasicProperties();
            bproperties.Headers = metadata ?? new Dictionary<string, object>();
            var json = JsonConvert.SerializeObject(request, Formatting.Indented,
                            new JsonSerializerSettings
                            {
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            });

            channel.BasicPublish(exchange: string.Empty,
                                 routingKey: QueueName,
                                 basicProperties: bproperties,
                                 body: Encoding.UTF8.GetBytes(json));
        }

        /// <summary>
        /// Permite desecolar un mensaje de una cola y al mensaje desencolado ser procesado por una funcion
        /// </summary>
        /// <typeparam name="TModel">Tipo de mensaje a encolar</typeparam>
        /// <param name="queue">Nombre de la cola a monitorear</param>
        /// <param name="code">Funcion que procesara el mensaje</param>
        /// <returns></returns>
        public async Task Dequeue<TModel>(Func<QueueMessage<TModel>, Task> code)
        {
            channel = Connection.CreateModel();
            channel.QueueDeclare(queue: QueueName,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                if (disposedValue == true)
                    throw new ObjectDisposedException(nameof(RabbitQueue));
                byte[] body = ea.Body.ToArray();
                var request = JsonConvert.DeserializeObject<TModel>(Encoding.UTF8.GetString(body));
                try
                {
                    IDictionary<string, object> headers = ea.BasicProperties?.Headers ?? new Dictionary<string, object>();
                    var headersBuilder = new Dictionary<string, string>();
                    foreach (var kv in headers)
                    {
#pragma warning disable CS8604 // Possible null reference argument.
                        var value = (kv.Value as byte[] != null)
                            ? Encoding.UTF8.GetString(kv.Value as byte[])
                            : kv.Value.ToString();
                        headersBuilder.Add(kv.Key, value);
                    }
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning disable CS8601 // Possible null reference assignment.
                    var msn = new QueueMessage<TModel>
                    {
                        Message = request,
                        Metadata = headersBuilder
                    };
#pragma warning restore CS8601 // Possible null reference assignment.

                    await code(msn);
                    channel.BasicAck(deliveryTag: ea.DeliveryTag,
                                    multiple: false);
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex, "Dequeue message error");
                    try
                    {
                        //En caso de suceder un error al procesar el mensaje de la cola, se dispondra en una cola de fallidos
                        var headers = ea.BasicProperties?.Headers ?? new Dictionary<string, object>();
                        var iretries = default(int);
                        _ = headers.TryGetValue("x-retries", out object? oretries) && int.TryParse(oretries?.ToString(), out iretries);
                        iretries++;
                        using var channel2 = Connection.CreateModel();
                        var queueName2 = QueueName + ((iretries >= MaxNotificationAttempts) ? "-dead-letter" : string.Empty);
                        channel2.QueueDeclare(queue: queueName2,
                                                  durable: true,
                                                  exclusive: false,
                                                  autoDelete: false,
                                                  arguments: null);
                        var bproperties = channel2.CreateBasicProperties();
                        bproperties.Headers = new Dictionary<string, object>
                        {
                            ["x-retries"] = iretries,
                            ["x-exception"] = ex.Message
                        };
                        if (queueName2 == QueueName)
                            await Task.Delay(TimeSpan.FromMinutes(1));
                        channel2.BasicPublish(exchange: string.Empty,
                                         routingKey: queueName2,
                                         basicProperties: bproperties,
                                         body: Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request)));
                        channel.BasicAck(deliveryTag: ea.DeliveryTag,
                                 multiple: false);
                    }
                    catch (Exception ex2)
                    {
                        //En caso de suceder un error al reencolar el mensaje en una cola de error se guarda la excepcion
                        Logger.LogError(ex2, "Requeue message error");
                        channel.BasicNack(deliveryTag: ea.DeliveryTag,
                                          multiple: false,
                                          requeue: true);
                    }
                }
            };
            ConsumerTag = this.channel.BasicConsume(queue: QueueName,
                                                   autoAck: false,
                                                   consumer: consumer);
            await Task.CompletedTask;
        }

        #region Soporte a IDisposable
        /// <summary>
        /// Variable empleada para detectar llamadas redundantes
        /// </summary>
        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (channel != null)
                    {
                        ConsumerTag = null;
                        channel.Dispose();
                        channel = null;
                    }
                }

                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
