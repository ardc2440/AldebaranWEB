using Aldebaran.Application.Services;
using Aldebaran.DataAccess.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Aldebaran.Web.Controllers
{
    [Route("Notification/[action]")]
    public class NotificationController : Controller
    {
        private readonly IPurchaseOrderNotificationService PurchaseOrderNotificationService;
        public NotificationController(IPurchaseOrderNotificationService purchaseOrderNotificationService)
        {
            PurchaseOrderNotificationService = purchaseOrderNotificationService;
        }
        [HttpPost]
        public async Task<IActionResult> PurchaseOrderUpdate([FromBody] Application.Services.Notificator.Model.MessageModel message, CancellationToken ct = default)
        {
            var notificationId = message.Header.MessageUid;
            await PurchaseOrderNotificationService.UpdateNotificationResponseAsync(notificationId, message.MessageDeliveryStatus.Success ? NotificationStatus.Success : NotificationStatus.Error, message.MessageDeliveryStatus.Message, message.Header.SentDate.Value, ct);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> CustomerOrderUpdate([FromBody] Application.Services.Notificator.Model.MessageModel message, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<IActionResult> CustomerReservationUpdate([FromBody] Application.Services.Notificator.Model.MessageModel message, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}