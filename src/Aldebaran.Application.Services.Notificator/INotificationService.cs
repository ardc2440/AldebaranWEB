namespace Aldebaran.Application.Services.Notificator
{
    public interface INotificationService
    {
        /// <summary>
        /// Envio de notificaciones
        /// </summary>
        /// <param name="message">Notificacion a enviar</param>
        /// <param name="ct">Token de cancelacion</param>
        /// <returns></returns>
        Task Send(Model.MessageModel message, CancellationToken ct = default);
        Task Send(Model.MessageModel message, string additionalBodyMessage, CancellationToken ct = default);
    }
}
