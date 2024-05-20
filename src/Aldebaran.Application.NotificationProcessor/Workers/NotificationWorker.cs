using Aldebaran.Application.NotificationProcessor.Services;
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
        /// </summary>
        /// <param name="queuer">Manejador de colas</param>
        /// <param name="serviceProvider">Proveedor de servicios</param>
        /// <param name="notificationProvider">Servicio de notificaciones</param>
        public NotificationWorker(IQueue queuer, IServiceProvider serviceProvider, ILogger<NotificationWorker> logger)
        {
            _queuer = queuer ?? throw new ArgumentNullException(nameof(IQueue));
            _logger = logger ?? throw new ArgumentNullException(nameof(ILogger<NotificationWorker>));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(IServiceProvider));
        }
        /// <summary>
        /// Inicio del procesamiento de notificaciones
        /// </summary>
        /// <param name="ct">Token de cancelacion</param>
        /// <returns></returns>
        public async Task StartAsync(CancellationToken ct)
        {
            try
            {
                await NotificationBrokerAsync(ct);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogCritical(ex, "Error al inciar NotificationWorker");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sin identificar en NotificationWorker");
            }
        }
        async Task NotificationBrokerAsync(CancellationToken ct = default)
        {
            await _queuer.Dequeue<MessageModel>(async data =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    if (scope == null)
                    {
                        _logger.LogCritical("Scope es null cuando no deberia serlo en NotificationBrokerAsync");
                        return;
                    }
                    var notificationProvider = scope.ServiceProvider.GetRequiredService<INotificationProvider>();
                    var message = data.Message;
                    var metadata = data.Metadata;
                    try
                    {
                        notificationProvider.Configure(metadata);
                        message.Header.SentDate = DateTime.Now;
                        await notificationProvider.SendMessage(message, metadata, ct);
                        message.MessageDeliveryStatus = new MessageModel.DeliveryStatus
                        {
                            Status = 200,
                            Success = true
                        };
                        _logger.LogInformation($"Notificacion {message.Header.MessageUid} ha sido enviada.");
                    }
                    catch (Exception ex)
                    {
                        message.MessageDeliveryStatus = new MessageModel.DeliveryStatus
                        {
                            Status = 400,
                            Success = false,
                            Message = ex.Message
                        };
                        _logger.LogError(ex, "NotificationWorker Service");
                    }
                    finally
                    {
                        await NotificationResultBrokerAsync(message, scope, ct);
                    }
                }
            });
        }

        async Task NotificationResultBrokerAsync(MessageModel message, IServiceScope scope, CancellationToken ct = default)
        {
            if (message.HookUrl == null)
            {
                _logger.LogInformation($"El mensaje no tiene un HookUrl válido para reportar el estado de la notificación.");
                return;
            }
            var hookApi = scope.ServiceProvider.GetRequiredService<IClientHookApi>();
            try
            {
                hookApi.Client.BaseAddress = message.HookUrl;
                await hookApi.SendMessageStatus(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ha ocurrido un error al intentar reportar el estado de la notificación al hook {message.HookUrl} [{DateTime.Now}]");
            }
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _queuer.Dispose();
            return Task.CompletedTask;
        }
    }
}
