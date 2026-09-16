using Aldebaran.Application.Services.Notifications.Models;
using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Infraestructure.Repository;


namespace Aldebaran.Application.Services.Notifications
{
    public class NotificationProcessingService : INotificationProcessingService
    {
        private readonly INotificationDefinitionRepository _notificationDefinitionRepository;

        private readonly INotificationDispatcher _notificationDispatcher;

        public NotificationProcessingService(INotificationDefinitionRepository notificationDefinitionRepository, INotificationDispatcher notificationDispatcher)
        {
            _notificationDefinitionRepository = notificationDefinitionRepository;

            _notificationDispatcher = notificationDispatcher;
        }

        public async Task<ICollection<NotificationProcessingResult>> ProcessNotificationsAsync(CancellationToken ct = default)
        {
            var results = new List<NotificationProcessingResult>();
            var notifications = new List<NotificationEvent>();

            var definitions = await _notificationDefinitionRepository.GetActiveAsync(ct);

            foreach (var definition in definitions)
            {
                if (!ShouldValidate(definition)) continue;

                try
                {
                    var result = await _notificationDefinitionRepository.ExecuteValidationQueryAsync(definition.ValidationQuery, ct);

                    if (result > 0)
                    {
                        var roles = await _notificationDefinitionRepository.GetRolesAsync(definition.NotificationDefinitionId, ct);

                        notifications.Add(
                            new NotificationEvent
                            {
                                NotificationDefinitionId = definition.NotificationDefinitionId,
                                Name = definition.Name,
                                NotificationMessage = definition.NotificationMessage,
                                QueryParameters = definition.QueryParameters,
                                Roles = roles.ToList()
                            });
                    }

                    results.Add(
                        new NotificationProcessingResult
                        {
                            NotificationDefinitionId = definition.NotificationDefinitionId,
                            NotificationName = definition.Name,
                            Success = true,
                            RecordsFound = result,
                            NotificationPublished = result > 0
                        });
                }
                catch (Exception ex)
                {
                    results.Add(
                        new NotificationProcessingResult
                        {
                            NotificationDefinitionId = definition.NotificationDefinitionId,
                            NotificationName = definition.Name,
                            Success = false,
                            ErrorMessage = ex.Message
                        });
                }
                finally
                {
                    await _notificationDefinitionRepository.UpdateLastValidationDateAsync(definition.NotificationDefinitionId, DateTime.Now, ct);
                }
            }

            await _notificationDispatcher.PublishAsync(notifications, ct);

            return results;
        }

        private static bool ShouldValidate(NotificationDefinition definition)
        {
            if (!definition.LastValidationDate.HasValue)
                return true;

            return DateTime.Now >= definition.LastValidationDate.Value.AddMinutes(definition.ValidationIntervalMinutes);
        }
    }
}
