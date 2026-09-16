using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services.Models.Enums;
using Aldebaran.Application.Services.NotificationsAccessToken;
using Aldebaran.Infraestructure.Common.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aldebaran.Web.Controllers
{
    [Route("Notification/[action]")]
    public class NotificationController : Controller
    {
        private readonly IPurchaseOrderNotificationService PurchaseOrderNotificationService;
        private readonly ICustomerOrderNotificationService CustomerOrderNotificationService;
        private readonly ICustomerReservationNotificationService CustomerReservationNotificationService;
        private readonly IEncryptionService EncryptionService;
        private readonly INotificationAccessTokenService _notificationAccessTokenService;

        public NotificationController(
            IEncryptionService encryptionService,
            IPurchaseOrderNotificationService purchaseOrderNotificationService,
            ICustomerOrderNotificationService customerOrderNotificationService,
            ICustomerReservationNotificationService customerReservationNotificationService,
            INotificationAccessTokenService notificationAccessTokenService)
        {
            PurchaseOrderNotificationService = purchaseOrderNotificationService;
            CustomerOrderNotificationService = customerOrderNotificationService;
            CustomerReservationNotificationService = customerReservationNotificationService;
            EncryptionService = encryptionService;
            _notificationAccessTokenService = notificationAccessTokenService;
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
        public async Task<IActionResult> CustomerOrderUpdate([FromBody] Application.Services.Notificator.Model.MessageModel message, CancellationToken ct = default)
        {
            var notificationId = message.Header.MessageUid;
            var status = message.MessageDeliveryStatus.Success ? NotificationStatus.Success : NotificationStatus.Error;
            await CustomerOrderNotificationService.UpdateAsync(notificationId, status, message.MessageDeliveryStatus.Message, message.Header.SentDate.Value, ct);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CustomerReservationUpdate([FromBody] Application.Services.Notificator.Model.MessageModel message, CancellationToken ct = default)
        {
            var notificationId = message.Header.MessageUid;
            var status = message.MessageDeliveryStatus.Success ? NotificationStatus.Success : NotificationStatus.Error;
            await CustomerReservationNotificationService.UpdateAsync(notificationId, status, message.MessageDeliveryStatus.Message, message.Header.SentDate.Value, ct);
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> MarkMinimumQuantityAlarmsAsRead(string token, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(token)) return RedirectToAction(nameof(NotificationResult), new { result = "invalid" });

            Guid tokenId;

            try
            {
                var decryptedToken = EncryptionService.Decrypt(token);
                tokenId = Guid.Parse(decryptedToken);
            }
            catch
            {
                return RedirectToAction(nameof(NotificationResult), new { result = "invalid" });
            }

            var notificationToken = await _notificationAccessTokenService.ValidateAsync(tokenId, ct);

            switch (notificationToken)
            {
                case NotificationTokenValidationResult.NotFound:
                    return RedirectToAction(nameof(NotificationResult), new { result = "invalid" });
                case NotificationTokenValidationResult.Expired:
                    return RedirectToAction(nameof(NotificationResult), new { result = "expired" });
                case NotificationTokenValidationResult.Consumed:
                    return RedirectToAction(nameof(NotificationResult), new { result = "used" });
            }

            if (notificationToken == NotificationTokenValidationResult.Valid)
            {
                if (NotificationTokenValidationResult.Success == await _notificationAccessTokenService.ConsumeAsync(tokenId, ct))
                    return RedirectToAction(nameof(NotificationResult), new { result = "success" });
            }

            return RedirectToAction(nameof(NotificationResult), new { result = "invalid" });
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult NotificationResult(string result)
        {
            return result switch
            {
                "success" => Success(),
                "expired" => ExpiredToken(),
                "used" => AlreadyUsedToken(),
                _ => InvalidToken()
            };
        }

        private ContentResult InvalidToken()
        {
            return Content(
                """
        <!DOCTYPE html>
        <html>
            <head>
                <meta charset="utf-8" />
                <title>Enlace inválido</title>
            </head>
            <body style="font-family:Arial;padding:40px">
                <h2>❌ Enlace inválido</h2>

                <p>
                    El enlace suministrado no es válido o ha sido alterado.
                </p>
            </body>
        </html>
        """,
                "text/html; charset=utf-8");
        }

        private ContentResult ExpiredToken()
        {
            return Content(
                """
        <!DOCTYPE html>
        <html>
            <head>
                <meta charset="utf-8" />
                <title>Enlace expirado</title>
            </head>
            <body style="font-family:Arial;padding:40px">
                <h2>⌛ Enlace expirado</h2>

                <p>
                    El enlace ya no se encuentra vigente.
                </p>
            </body>
        </html>
        """,
                "text/html; charset=utf-8");
        }

        private ContentResult AlreadyUsedToken()
        {
            return Content(
                """
        <!DOCTYPE html>
        <html>
            <head>
                <meta charset="utf-8" />
                <title>Enlace utilizado</title>
            </head>
            <body style="font-family:Arial;padding:40px">
                <h2>⚠️ Enlace utilizado</h2>

                <p>
                    Este enlace ya fue utilizado anteriormente.
                </p>
            </body>
        </html>
        """,
                "text/html; charset=utf-8");
        }

        private ContentResult Success()
        {
            return Content(
                """
        <!DOCTYPE html>
        <html>
            <head>
                <meta charset="utf-8" />
                <title>Proceso completado</title>
            </head>
            <body style="font-family:Arial;padding:40px">
                <h2>✅ Proceso completado</h2>

                <p>
                     Las alarmas fueron marcadas como leídas correctamente.
                </p>
            </body>
        </html>
        """,
                "text/html; charset=utf-8");
        }
    }
}