using Aldebaran.Application.Services.Notificator.Model;
using Aldebaran.DataAccess.Infraestructure.Repository;
using Aldebaran.Infraestructure.Core.Queue;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Aldebaran.Application.Services.Notificator
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationProviderSettingsRepository _settingsRepository;
        private readonly INotificationTemplateRepository _templateRepository;
        private IDictionary<string, object>? MetaData = null;
        private readonly ILogger<NotificationService> _logger;
        /// <summary>
        /// Dependencia de Queuer
        /// </summary>
        private readonly IQueue _queuer;
        /// <summary>
        /// </summary>
        /// <param name="settingsRepository">Repositorio de configuracion de notificaciones</param>
        /// <param name="templateRepository">Repositorio de plantillas para notificaciones</param>
        /// <param name="queuer">Servicio de colas</param>
        /// <exception cref="ArgumentNullException"></exception>
        public NotificationService(INotificationProviderSettingsRepository settingsRepository, INotificationTemplateRepository templateRepository, IQueue queuer, ILogger<NotificationService> logger)
        {
            _settingsRepository = settingsRepository ?? throw new ArgumentNullException(nameof(INotificationProviderSettingsRepository));
            _templateRepository = templateRepository ?? throw new ArgumentNullException(nameof(INotificationTemplateRepository));
            _queuer = queuer ?? throw new ArgumentNullException(nameof(IQueue));
            _logger = logger ?? throw new ArgumentNullException(nameof(ILogger<NotificationService>));
        }

        public async Task Send(MessageModel message, CancellationToken ct = default)
        {
            var settings = await _settingsRepository.FindAsync(message.Header.Subject, ct);
            if (settings == null)
                throw new InvalidOperationException($"No existe una configuracion para {message.Header.Subject}");
            var template = await _templateRepository.FindAsync(message.Body.Template, ct);
            if (template == null)
                throw new InvalidOperationException($"Plantilla no existe {message.Body.Template}");

            message.Body.Subject = template.Subject;
            message.Body.Message = template.Message;

            var providerParameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(settings.Settings);
            MetaData = new Dictionary<string, object>();
            foreach (var kv in providerParameters)
                MetaData[kv.Key] = kv.Value;
            _queuer.Enqueue(message, MetaData);
        }

        public async Task Send(MessageModel message, string additionalBodyMessage, CancellationToken ct = default)
        {
            var settings = await _settingsRepository.FindAsync(message.Header.Subject, ct);
            if (settings == null)
                throw new InvalidOperationException($"No existe una configuracion para {message.Header.Subject}");
            var template = await _templateRepository.FindAsync(message.Body.Template, ct);
            if (template == null)
                throw new InvalidOperationException($"Plantilla no existe {message.Body.Template}");

            message.Body.Subject = template.Subject;
            message.Body.Message = template.Message + additionalBodyMessage;

            var providerParameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(settings.Settings);
            MetaData = new Dictionary<string, object>();
            foreach (var kv in providerParameters)
                MetaData[kv.Key] = kv.Value;
            _queuer.Enqueue(message, MetaData);
        }
    }
}