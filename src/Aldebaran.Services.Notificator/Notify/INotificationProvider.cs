using Aldebaran.Services.Notificator.Notify.Model;

namespace Aldebaran.Services.Notificator.Notify
{
    /// <summary>
    /// Contrato para enviar una notificacion
    /// </summary>
    public interface INotificationProvider
    {
        /// <summary>
        /// Lista de argumentos que deberan ser suministrados al momento de configurar el proveedor
        /// <see cref="INotificationProvider.Configure(string, Dictionary{string, string})"/>
        /// </summary>
        string[] ProviderArguments { get; }
        /// <summary>
        /// Establece los argumentos para necesarios para la autenticacion contra el proveedor
        /// </summary>
        /// <param name="parameters">Argumentos de configuracion necesarios que usara el proveedor para el envio de notificaciones</param>
        void Configure(Dictionary<string, string> parameters);
        /// <summary>
        /// Envio de la notificacion
        /// </summary>
        /// <param name="message">Modelo de mensaje a enviar</param>
        /// <param name="ct">Token de cancelacion asyncronico</param>
        /// <returns></returns>
        Task SendMessage(MessageModel message, IDictionary<string, string> metadata, CancellationToken ct = default);
    }
}
