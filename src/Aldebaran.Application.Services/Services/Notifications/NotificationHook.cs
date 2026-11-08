using System.Security.Cryptography;
using System.Text;
using Aldebaran.Application.Services.Notificator.Model;

namespace Aldebaran.Application.Services.Notifications
{
    /// <summary>Endpoints del sitio que reciben el estado de envío de una notificación (POST del procesador de notificaciones).</summary>
    public enum NotificationHook
    {
        PurchaseOrderUpdate,
        CustomerOrderUpdate,
        CustomerReservationUpdate
    }

    /// <summary>Resultado de validar un POST de estado de notificación.</summary>
    public enum NotificationHookCheck
    {
        Valid,
        /// <summary>Mensaje incompleto: 400.</summary>
        Malformed,
        /// <summary>Firma ausente o que no corresponde al MessageUid: 401.</summary>
        InvalidSignature
    }

    /// <summary>
    /// Firma de la URL de retorno (HookUrl) de las notificaciones. El procesador de notificaciones hace POST a la URL que
    /// trae el mensaje, sin credenciales; la firma (HMAC-SHA256 del MessageUid con una clave que solo conoce el sitio) va
    /// en la ruta, así el procesador no necesita cambios y nadie sin la clave puede alterar el estado de una notificación.
    /// </summary>
    public interface INotificationHookSigner
    {
        /// <summary>URL de retorno firmada: {baseUri}/Notification/{hook}/{firma}.</summary>
        Uri BuildHookUrl(string baseUri, NotificationHook hook, string messageUid);

        /// <summary>Valida que el mensaje esté completo y que la firma corresponda a su MessageUid.</summary>
        NotificationHookCheck Check(MessageModel? message, string? signature);
    }

    public sealed class NotificationHookSigner : INotificationHookSigner
    {
        /// <summary>Longitud mínima de la clave (caracteres).</summary>
        public const int MinimumKeyLength = 32;
        private readonly byte[] _key;

        public NotificationHookSigner(string? key)
        {
            if (string.IsNullOrWhiteSpace(key) || key.Length < MinimumKeyLength)
                throw new InvalidOperationException($"Falta la clave AppSettings:NotificationHookKey o tiene menos de {MinimumKeyLength} caracteres.");
            _key = Encoding.UTF8.GetBytes(key);
        }

        public Uri BuildHookUrl(string baseUri, NotificationHook hook, string messageUid)
        {
            if (string.IsNullOrWhiteSpace(baseUri))
                throw new ArgumentException("La URL base es obligatoria.", nameof(baseUri));
            if (string.IsNullOrWhiteSpace(messageUid))
                throw new ArgumentException("El identificador del mensaje es obligatorio.", nameof(messageUid));
            return new Uri($"{baseUri.TrimEnd('/')}/Notification/{hook}/{Sign(messageUid)}");
        }

        public NotificationHookCheck Check(MessageModel? message, string? signature)
        {
            if (message?.Header == null || string.IsNullOrWhiteSpace(message.Header.MessageUid)
                || message.Header.SentDate == null || message.MessageDeliveryStatus == null)
                return NotificationHookCheck.Malformed;
            if (string.IsNullOrWhiteSpace(signature))
                return NotificationHookCheck.InvalidSignature;
            var expected = Encoding.ASCII.GetBytes(Sign(message.Header.MessageUid));
            var received = Encoding.ASCII.GetBytes(signature);
            return CryptographicOperations.FixedTimeEquals(expected, received) ? NotificationHookCheck.Valid : NotificationHookCheck.InvalidSignature;
        }

        /// <summary>HMAC-SHA256 en Base64 URL (sin relleno), apto para un segmento de ruta.</summary>
        internal string Sign(string messageUid)
        {
            var hash = HMACSHA256.HashData(_key, Encoding.UTF8.GetBytes(messageUid));
            return Convert.ToBase64String(hash).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }
    }
}
