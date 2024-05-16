using Aldebaran.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Aldebaran.Web.Controllers
{
    [Route("Notification/[action]")]
    public class NotificationController : Controller
    {
        private readonly IPurchaseOrderNotificationService PurchaseOrderNotificationService;
        private readonly ICustomerOrderNotificationService CustomerOrderNotificationService;
        private readonly ICustomerReservationNotificationService CustomerReservationNotificationService;


        [HttpPost]
        public async Task<IActionResult> PurchaseOrderUpdateAsync(Application.Services.Notificator.Model.MessageModel message, CancellationToken ct = default)
        {
            var template = message.Body.Template;
            var notificationId = message.Header.MessageUid;
            var status = message.MessageDeliveryStatus.Success ? Application.Services.Models.NotificationStatus.Success : Application.Services.Models.NotificationStatus.Error;
            await PurchaseOrderNotificationService.UpdateNotificationStatusAsync(notificationId, status, message.MessageDeliveryStatus.Message, ct);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> CustomerOrderUpdateAsync(Application.Services.Notificator.Model.MessageModel message, CancellationToken ct = default)
        {
            var template = message.Body.Template;
            var notificationId = message.Header.MessageUid;
            var status = message.MessageDeliveryStatus.Success ? Application.Services.Models.NotificationStatus.Success : Application.Services.Models.NotificationStatus.Error;
            await CustomerOrderNotificationService.UpdateAsync(notificationId, status, message.MessageDeliveryStatus.Message, ct);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CustomerReservationUpdateAsync(Application.Services.Notificator.Model.MessageModel message, CancellationToken ct = default)
        {
            var template = message.Body.Template;
            var notificationId = message.Header.MessageUid;
            var status = message.MessageDeliveryStatus.Success ? Application.Services.Models.NotificationStatus.Success : Application.Services.Models.NotificationStatus.Error;
            await CustomerReservationNotificationService.UpdateAsync(notificationId, status, message.MessageDeliveryStatus.Message, ct);
            return Ok();
        }
    }
}