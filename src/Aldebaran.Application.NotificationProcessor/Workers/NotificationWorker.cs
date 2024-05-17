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
        /// Dependencia de Proveedor de servicios
        /// </summary>
        private readonly INotificationProvider _notificationProvider;
        private readonly IQueueSettings _queueSettings;
        private readonly IClientHookApi _hookApi;
        /// <summary>
        /// </summary>
        /// <param name="queuer">Manejador de colas</param>
        /// <param name="serviceProvider">Proveedor de servicios</param>
        /// <param name="notificationProvider">Servicio de notificaciones</param>
        public NotificationWorker(IClientHookApi hookApi, IQueueSettings queueSettings, IQueue queuer, IServiceProvider serviceProvider, INotificationProvider notificationProvider, ILogger<NotificationWorker> logger)
        {
            _queuer = queuer ?? throw new ArgumentNullException(nameof(IQueue));
            _logger = logger ?? throw new ArgumentNullException(nameof(ILogger<NotificationWorker>));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(IServiceProvider));
            _notificationProvider = notificationProvider ?? throw new ArgumentNullException(nameof(INotificationProvider));
            _queueSettings = queueSettings ?? throw new ArgumentNullException(nameof(IQueueSettings));
            _hookApi = hookApi ?? throw new ArgumentNullException(nameof(IClientHookApi));
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
                await NotificationResultBrokerAsync(ct);
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
                    var message = data.Message;
                    var metadata = data.Metadata;
                    try
                    {
                        _notificationProvider.Configure(metadata);
                        await _notificationProvider.SendMessage(message, metadata, ct);
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
                        _queuer.Enqueue(_queueSettings.NotificationResultQueue, message);
                    }
                }
            });
        }
        async Task NotificationResultBrokerAsync(CancellationToken ct = default)
        {
            await _queuer.Dequeue<MessageModel>(_queueSettings.NotificationResultQueue, async data =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var message = data.Message;
                    if (_hookApi.Client.BaseAddress == null)
                    {
                        _logger.LogInformation($"El mensaje no tiene un HookUrl válido para reportar el estado de la notificación.");
                        return;
                    }
                    try
                    {
                        await _hookApi.SendMessageStatus(message);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Ha ocurrido un error al intentar reportar el estado de la notificación al hook {_hookApi.Client.BaseAddress} [{DateTime.Now}]");
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
