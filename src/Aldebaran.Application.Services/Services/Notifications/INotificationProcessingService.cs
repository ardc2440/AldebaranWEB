using Aldebaran.Application.Services.Notifications.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.Application.Services.Notifications
{
    public interface INotificationProcessingService
    {
        Task<ICollection<NotificationProcessingResult>> ProcessNotificationsAsync(CancellationToken ct = default);
    }
}
