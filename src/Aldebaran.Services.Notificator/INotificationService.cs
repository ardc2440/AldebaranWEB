using Aldebaran.Services.Notificator.Notify.Model;

namespace Aldebaran.Services.Notificator
{
    public interface INotificationService
    {
        /// <summary>
        /// Envio de notificaciones
        /// </summary>
        /// <param name="message">Notificacion a enviar</param>
        /// <param name="ct">Token de cancelacion</param>
        /// <returns></returns>
        Task Send(MessageModel message, CancellationToken ct = default);
    }
}
