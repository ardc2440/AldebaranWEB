using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.Application.Services.NotificationsAccessToken
{
    public interface INotificationAccessTokenHandler
    {
        string NotificationTemplateName { get; }

        Task<bool> ExecuteAsync(string? extraData, CancellationToken ct = default);
    }
}
