using Aldebaran.Application.Services.NotificationsAccessToken;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.Application.Services.NotificationsAccessToken
{
    public interface INotificationAccessTokenHandlerFactory
    {
        INotificationAccessTokenHandler GetHandler(string notificationTemplateName);
    }
}
