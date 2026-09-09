using Aldebaran.Application.Services.Notifications;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Aldebaran.Web.Services.Notifications
{
    public class NotificationWorker : BackgroundService
    {
        private readonly INotificationProcessingService _notificationProcessingService;
        private readonly ILogger<NotificationWorker> _logger;

        public NotificationWorker(INotificationProcessingService notificationProcessingService, ILogger<NotificationWorker> logger)
        {
            _notificationProcessingService = notificationProcessingService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var results = await _notificationProcessingService.ProcessNotificationsAsync(stoppingToken);

                    foreach (var result in results)
                        if (!result.Success)
                            _logger.LogError("Error procesando NotificationDefinitionId {NotificationDefinitionId}. Error: {ErrorMessage}",
                                result.NotificationDefinitionId,
                                result.ErrorMessage);
                        
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error general ejecutando NotificationWorker.");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}