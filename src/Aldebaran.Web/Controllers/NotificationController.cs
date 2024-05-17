using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace Aldebaran.Web.Controllers
{
    [Route("Notification/[action]")]
    public class NotificationController : Controller
    {
        private readonly IPurchaseOrderNotificationService PurchaseOrderNotificationService;
        private readonly ICustomerOrderNotificationService CustomerOrderNotificationService;
        private readonly ICustomerReservationNotificationService CustomerReservationNotificationService;

        public NotificationController(IPurchaseOrderNotificationService purchaseOrderNotificationService, ICustomerOrderNotificationService customerOrderNotificationService, ICustomerReservationNotificationService customerReservationNotificationService)
        {
            PurchaseOrderNotificationService = purchaseOrderNotificationService;
            CustomerOrderNotificationService = customerOrderNotificationService;
            CustomerReservationNotificationService = customerReservationNotificationService;
        }
        [HttpPost]
        public async Task<IActionResult> PurchaseOrderUpdate([FromBody] Application.Services.Notificator.Model.MessageModel message, CancellationToken ct = default)
        {
            var notificationId = message.Header.MessageUid;
            var status = message.MessageDeliveryStatus.Success ? NotificationStatus.Success : NotificationStatus.Error;
            await PurchaseOrderNotificationService.UpdateNotificationResponseAsync(notificationId, status, message.MessageDeliveryStatus.Message, message.Header.SentDate.Value, ct);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> CustomerOrderUpdateAsync(Application.Services.Notificator.Model.MessageModel message, CancellationToken ct = default)
        {
            var notificationId = message.Header.MessageUid;
            var status = message.MessageDeliveryStatus.Success ? NotificationStatus.Success : NotificationStatus.Error;
            await CustomerOrderNotificationService.UpdateAsync(notificationId, status, message.MessageDeliveryStatus.Message, ct);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CustomerReservationUpdateAsync(Application.Services.Notificator.Model.MessageModel message, CancellationToken ct = default)
        {
            var notificationId = message.Header.MessageUid;
            var status = message.MessageDeliveryStatus.Success ? Application.Services.Models.NotificationStatus.Success : Application.Services.Models.NotificationStatus.Error;
            await CustomerReservationNotificationService.UpdateAsync(notificationId, status, message.MessageDeliveryStatus.Message, ct);
            return Ok();
        }
    }
}