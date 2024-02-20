using Aldebaran.Application.Services.Notificator.Model;
using Aldebaran.Application.Services.Notificator.Notify;
using Aldebaran.Infraestructure.Core.Queue;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Aldebaran.Application.NotificationProcessor.Workers
{
    internal class NotificationWorker : IHostedService
    {
        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger<NotificationWorker> _logger;
        /// <summary>
        /// Dependencia de Queuer
        /// </summary>
        private readonly IQueue _queuer;
        /// <summary>
        /// Dependencia de Proveedor de servicios
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Dependencia de Proveedor de servicios
        /// </summary>
        private readonly INotificationProvider _notificationProvider;
        /// <summary>
        /// </summary>
        /// <param name="queuer">Manejador de colas</param>
        /// <param name="serviceProvider">Proveedor de servicios</param>
        /// <param name="notificationProvider">Servicio de notificaciones</param>
        public NotificationWorker(IQueue queuer, IServiceProvider serviceProvider, INotificationProvider notificationProvider, ILogger<NotificationWorker> logger)
        {
            _queuer = queuer ?? throw new ArgumentNullException(nameof(IQueue));
            _logger = logger ?? throw new ArgumentNullException(nameof(ILogger<NotificationWorker>));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(IServiceProvider));
            _notificationProvider = notificationProvider ?? throw new ArgumentNullException(nameof(INotificationProvider));
        }
        /// <summary>
        /// Inicio del procesamiento de notificaciones
        /// </summary>
        /// <param name="ct">Token de cancelacion</param>
        /// <returns></returns>
        public async Task StartAsync(CancellationToken ct)
        {
            await _queuer.Dequeue<MessageModel>(async data =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var message = data.Message;
                    var metadata = data.Metadata;
                    try
                    {
                        _notificationProvider.Configure(metadata);
                        await _notificationProvider.SendMessage(message, metadata, ct);
                        _logger.LogInformation($"Notificacion {message.Header.MessageUid} ha sido enviada.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "NotificationWorker Service");
                    }
                }
            });
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _queuer.Dispose();
            return Task.CompletedTask;
        }
    }
}
