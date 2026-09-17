using Aldebaran.Application.Services.InventoryMinimumAlerts;
using Aldebaran.Application.Services.InventoryMinimumAlerts.Models;
using Aldebaran.Web.Models;
using Microsoft.Extensions.Options;

namespace Aldebaran.Web.Services.InventoryMinimumAlert
{
    public class NotificationWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IOptions<InventoryMinimumAlertSettings> _inventorySettings;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly ILogger<NotificationWorker> _logger;

        public NotificationWorker(IServiceProvider serviceProvider, IOptions<InventoryMinimumAlertSettings> inventorySettings, IOptions<AppSettings> appSettings, ILogger<NotificationWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _inventorySettings = inventorySettings;
            _appSettings = appSettings;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_inventorySettings.Value.Enabled)
            {
                _logger.LogInformation("InventoryMinimumAlert deshabilitado.");
                return;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var nextExecution = GetNextExecution();

                    _logger.LogInformation("Próxima ejecución programada: {Date}", nextExecution);

                    var delay = nextExecution - DateTime.Now;

                    if (delay <= TimeSpan.Zero)                    
                        _logger.LogWarning("La fecha calculada ya expiró. Se ejecutará inmediatamente.");
                    else
                        await Task.Delay(delay, stoppingToken);
                    
                    using var scope = _serviceProvider.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<IInventoryMinimumAlertService>();

                    _logger.LogInformation("Ejecutando InventoryMinimumAlertService");
                    await service.ExecuteAsync(_appSettings.Value.ImageRepositoryPath, stoppingToken);
                    _logger.LogInformation("InventoryMinimumAlertService finalizado");
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error ejecutando InventoryMinimumAlertWorker");

                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
        }

        private DateTime GetNextExecution()
        {
            var candidates = new List<DateTime>();

            foreach (var day in _inventorySettings.Value.DaysOfWeek)
                foreach (var hour in _inventorySettings.Value.ExecutionHours)
                    candidates.Add(GetNextOccurrence(day, hour));
                            
            return candidates.Min();
        }

        private static DateTime GetNextOccurrence(DayOfWeek day, TimeSpan hour)
        {
            var now = DateTime.Now;
            var date = now.Date.AddDays(((int)day - (int)now.DayOfWeek + 7) % 7);

            date = date.Add(hour);
            if (date <= now) date = date.AddDays(7);

            return date;
        }
    }
}
