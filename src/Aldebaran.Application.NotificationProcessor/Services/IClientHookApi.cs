using Aldebaran.Application.Services.Notificator.Model;
using Refit;

namespace Aldebaran.Application.NotificationProcessor.Services
{
    public interface IClientHookApi
    {
        HttpClient Client { get; }
        /// <summary>
        /// Envio de estado del envio de la notificacion
        /// </summary>
        /// <param name="mr">Notificacion procesada</param>
        /// <returns>Modelo de mensaje de respuesta</returns>
        [Post("")]
        Task SendMessageStatus([Body] MessageModel mr);
    }
}
