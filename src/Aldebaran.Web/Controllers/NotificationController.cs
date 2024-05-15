using Aldebaran.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Aldebaran.Web.Controllers
{
    [Route("Notification/[action]")]
    public class NotificationController : Controller
    {
        private readonly IPurchaseOrderNotificationService PurchaseOrderNotificationService;

        [HttpPost]
        public async Task<IActionResult> PurchaseOrderUpdate(Application.Services.Notificator.Model.MessageModel message, CancellationToken ct = default)
        {
            var template = message.Body.Template;
            var notificationId = message.Header.MessageUid;
            var status = message.MessageDeliveryStatus.Success ? Application.Services.Models.NotificationStatus.Success : Application.Services.Models.NotificationStatus.Error;
            await PurchaseOrderNotificationService.UpdateNotificationStatusAsync(notificationId, status, message.MessageDeliveryStatus.Message, ct);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> CustomerOrderUpdate(Application.Services.Notificator.Model.MessageModel message, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<IActionResult> CustomerReservationUpdate(Application.Services.Notificator.Model.MessageModel message, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}