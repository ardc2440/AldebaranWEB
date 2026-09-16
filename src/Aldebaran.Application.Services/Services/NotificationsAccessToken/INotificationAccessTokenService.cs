using Aldebaran.Application.Services.Models.Enums;
using Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services.NotificationsAccessToken
{
    public interface INotificationAccessTokenService
    {
        Task<bool> AddAsync(int employeeId, short templateId, List<int> alarmIds, Guid tokenId, CancellationToken ct = default);
        Task<NotificationAccessToken?> FindAsync(Guid tokenId, CancellationToken ct = default);
        Task<NotificationTokenValidationResult> ConsumeAsync(Guid tokenId, CancellationToken ct = default);
        Task<NotificationTokenValidationResult> ValidateAsync(Guid tokenId, CancellationToken ct = default);
    }

}
