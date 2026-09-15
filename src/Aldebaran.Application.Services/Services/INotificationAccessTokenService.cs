using Aldebaran.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.Application.Services.Services
{
    public interface INotificationAccessTokenService
    {
        Task<bool> AddAsync(int employeeId, short templateId, List<int> alarmIds, Guid tokenId, CancellationToken ct = default);
        Task<NotificationAccessToken?> FindAsync(Guid tokenId, CancellationToken ct = default);
        Task<bool> ConsumeAsync(Guid tokenId, CancellationToken ct = default);
    }

}
