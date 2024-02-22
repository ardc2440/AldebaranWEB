
using Aldebaran.Application.Services.Notificator.Model;
using Aldebaran.Application.Services.Notificator.Notify;
using Aldebaran.Infraestructure.Common.Extensions;
using MailKit.Security;

namespace Aldebaran.Application.Services.Notificator.EmailProvider
{
    public class EmailNotificationProvider : INotificationProvider
    {
        /// <summary>
        /// Dependencia de IEmailService
        /// </summary>
        private readonly Services.IEmailService _emailService;
        /// <summary>
        /// Parametros requeridos para el envio del correo
        /// </summary>
        public string[] ProviderArguments => new string[] {
            "mail_server",
            "sender_name",
            "sender_email",
            "password",
            "port",
            "secure_socket_option"
        };

        public EmailNotificationProvider(Services.IEmailService emailService)
        {
            _emailService = emailService ?? throw new ArgumentNullException(nameof(Services.IEmailService));
        }
        /// <summary>
        /// Establece los argumentos para necesarios para la autenticacion contra el proveedor
        /// </summary>
        /// <param name="parameters">Argumentos de configuracion necesarios que usara el proveedor para el envio de notificaciones</param>
        /// <exception cref="KeyNotFoundException">Cuando los <paramref name="parameters"/> no contienen toda la informacion necesaria de configuracion</exception>
        /// <exception cref="ArgumentException">Cuando la informacion porporcionada de los <paramref name="parameters"/> es invalida</exception>
        public void Configure(IDictionary<string, string> parameters)
        {
            //Se valida que todos los parametros pasados como argumento, sean los requeridos para la operacion
            foreach (var argument in ProviderArguments)
                if (!parameters.ContainsKey(argument))
                    throw new KeyNotFoundException($"{argument} not configured in Notificator parameters");
            //Puerto debe ser numerico y el
            var safePort = int.TryParse(parameters["port"], out int outPort) ? (int?)outPort : null;
            var port = safePort ?? throw new ArgumentException("Invalid port");
            //El tipo de socket debe ser valido
            if (!Enum.IsDefined(typeof(SecureSocketOptions), parameters["secure_socket_option"]))
                throw new ArgumentException("Invalid socketOption");

            var mailServer = parameters["mail_server"];
            var senderName = parameters["sender_name"];
            var senderEmail = parameters["sender_email"];
            var password = parameters["password"].Decrypt();
            var socketOption = Enum.Parse<SecureSocketOptions>(parameters["secure_socket_option"]);
            //Configuracion del proveedor que se usara para el envio de email
            _emailService.Configure(mailServer, port, socketOption, senderEmail, password, senderName);
        }

        /// <summary>
        /// Envio de la notificacion
        /// </summary>
        /// <param name="message">Modelo de mensaje a enviar</param>
        /// <param name="ct">Token de cancelacion asyncronico</param>
        /// <returns></returns>
        public async Task SendMessage(MessageModel message, IDictionary<string, string> metadata, CancellationToken ct = default)
        {
            var subject = message.Body.Subject;
            var body = message.Body.Message;
            var to = message.Header.ReceiverUrn;
            var cc = message.Header.ReceiverUrnCc;
            var bcc = message.Header.ReceiverUrnBcc;
            var attachments = message.Body.Medias?.Select(s => new Models.Attachment() { ContentType = s.ContentType, FileName = s.FileName, Hash = s.Hash }).ToArray();

            await _emailService.SendAsync(subject,
                body,
                to,
                cc,
                bcc,
                attachments,
                ct);
        }
    }
}
