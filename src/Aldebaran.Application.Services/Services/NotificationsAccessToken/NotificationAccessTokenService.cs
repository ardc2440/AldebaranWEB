using Aldebaran.Application.Services.Models.Enums;
using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Infraestructure.Repository;
using Newtonsoft.Json;

namespace Aldebaran.Application.Services.NotificationsAccessToken
{
    public class NotificationAccessTokenService : INotificationAccessTokenService
    {
        private readonly INotificationAccessTokenRepository _repository;
        private readonly INotificationAccessTokenHandlerFactory _factory;

        public NotificationAccessTokenService(INotificationAccessTokenHandlerFactory factory, INotificationAccessTokenRepository repository)
        {
            _factory = factory;
            _repository = repository;
        }

        public async Task<bool> AddAsync(int employeeId, short templateId, List<int> alarmIds, Guid tokenId, CancellationToken ct = default)
        {
            var entity = new DataAccess.Entities.NotificationAccessToken
            {
                NotificationAccessTokenId = tokenId,
                NotificationTemplateId = templateId,
                ExtraData = JsonConvert.SerializeObject(new { EmployeeId = employeeId, AlarmIds = alarmIds }),
                GeneratedDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddHours(12),
                IsConsumed = false
            };
            return await _repository.AddAsync(entity, ct);
        }

        public async Task<NotificationTokenValidationResult> ConsumeAsync(Guid tokenId, CancellationToken ct = default)
        {
            var token = await _repository.FindAsync(tokenId, ct);

            if (token == null) return NotificationTokenValidationResult.NotFound;

            var handler = _factory.GetHandler(token.NotificationTemplate.Name);

            var processed = await handler.ExecuteAsync(token.ExtraData, ct);

            if (!processed) return NotificationTokenValidationResult.Failed;

            await _repository.ConsumeAsync(tokenId, ct);

            return NotificationTokenValidationResult.Success;
        }

        public async Task<NotificationAccessToken?> FindAsync(Guid tokenId, CancellationToken ct = default)
        {
            return await _repository.FindAsync(tokenId, ct);
        }

        public async Task<NotificationTokenValidationResult> ValidateAsync(Guid tokenId, CancellationToken ct = default)
        {
            var notificationToken = await FindAsync(tokenId, ct);

            if (notificationToken is null) return NotificationTokenValidationResult.NotFound;

            if (notificationToken.ExpirationDate < DateTime.Now) return NotificationTokenValidationResult.Expired;

            if (notificationToken.IsConsumed) return NotificationTokenValidationResult.Consumed;

            return NotificationTokenValidationResult.Valid;
        }
    }
}
