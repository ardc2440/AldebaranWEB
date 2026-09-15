using Aldebaran.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface INotificationAccessTokenRepository
    {
        Task<bool> AddAsync(NotificationAccessToken token, CancellationToken ct = default);
        Task<NotificationAccessToken?> FindAsync(Guid tokenId, CancellationToken ct = default);
        Task<bool> ConsumeAsync(Guid tokenId, CancellationToken ct = default);
    }
}
