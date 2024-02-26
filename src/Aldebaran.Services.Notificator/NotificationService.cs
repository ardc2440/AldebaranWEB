using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Infraestructure.Repository;
using Aldebaran.Infraestructure.Core.Queue;
using Aldebaran.Services.Notificator.Notify.Model;

namespace Aldebaran.Services.Notificator
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationProviderSettingsRepository _settingsRepository;
        private IDictionary<string, object>? MetaData = null;
        /// <summary>
        /// Dependencia de Queuer
        /// </summary>
        private readonly IQueue _queuer;
        private NotificationProviderSetting? EmailSettings;
        /// <summary>
        /// </summary>
        /// <param name="settingsRepository">Repositorio de configuracion de notificaciones</param>
        /// <param name="queuer">Servicio de colas</param>
        /// <exception cref="ArgumentNullException"></exception>
        public NotificationService(INotificationProviderSettingsRepository settingsRepository, IQueue queuer)
        {
            _settingsRepository = settingsRepository ?? throw new ArgumentNullException(nameof(INotificationProviderSettingsRepository));
            _queuer = queuer ?? throw new ArgumentNullException(nameof(IQueue));
        }

        public async Task Send(MessageModel message, CancellationToken ct = default)
        {
            EmailSettings = await _settingsRepository.FindAsync(message.Header.Subject, ct);
            if (EmailSettings == null)
                throw new InvalidOperationException($"No existe una configuracion para {message.Header.Subject}");

            MetaData = new Dictionary<string, object>();
            var providerParameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(EmailSettings.Settings);
            foreach (var kv in providerParameters)
                MetaData[kv.Key] = kv.Value;

            message.Header.SentDate = DateTime.UtcNow;
            _queuer.Enqueue(message, MetaData);
        }
    }
}
