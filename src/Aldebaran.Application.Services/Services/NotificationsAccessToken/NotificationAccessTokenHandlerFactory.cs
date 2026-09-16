using Aldebaran.Application.Services.NotificationsAccessToken;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.Application.Services.NotificationsAccessToken
{
    public class NotificationAccessTokenHandlerFactory  : INotificationAccessTokenHandlerFactory
    {
        private readonly IEnumerable<INotificationAccessTokenHandler> _handlers;

        public NotificationAccessTokenHandlerFactory(IEnumerable<INotificationAccessTokenHandler> handlers)
        {
            _handlers = handlers;
        }

        public INotificationAccessTokenHandler GetHandler(string notificationTemplateName)
        {
            return _handlers.First(x => x.NotificationTemplateName == notificationTemplateName);
        }
    }
}
