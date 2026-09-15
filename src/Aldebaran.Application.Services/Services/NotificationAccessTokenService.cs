using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Infraestructure.Repository;
using Newtonsoft.Json;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.Application.Services.Services
{
    public class NotificationAccessTokenService : INotificationAccessTokenService
    {
        private readonly INotificationAccessTokenRepository _repository;
        public NotificationAccessTokenService(INotificationAccessTokenRepository repository)
        {
            _repository = repository;
        }

        public Task<bool> AddAsync(int employeeId, short templateId, List<int> alarmIds, Guid tokenId, CancellationToken ct = default)
        {
           var entity = new NotificationAccessToken
            {
                NotificationAccessTokenId = tokenId,
                NotificationTemplateId = templateId, 
                ExtraData = JsonConvert.SerializeObject(new { EmployeeId = employeeId, AlarmIds = alarmIds }),
                GeneratedDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddHours(12),
                IsConsumed = false
            };
            return _repository.AddAsync(entity, ct);
        }

        public Task<bool> ConsumeAsync(Guid tokenId, CancellationToken ct = default)
        {
            return _repository.ConsumeAsync(tokenId, ct);
        }

        public Task<NotificationAccessToken?> FindAsync(Guid tokenId, CancellationToken ct = default)
        {
            return _repository.FindAsync(tokenId, ct);
        }
    }
}
