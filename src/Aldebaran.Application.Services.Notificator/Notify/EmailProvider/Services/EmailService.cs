using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Aldebaran.Application.Services.Notificator.Services
{
    public class EmailService : IEmailService
    {
        /// <summary>
        /// Servidor de correos
        /// </summary>
        private string MailServer;
        /// <summary>
        /// Puerto del servidor de correos
        /// </summary>
        private int Port;
        /// <summary>
        /// Tipo de socket a utilizar para el envio del correo
        /// </summary>
        private SecureSocketOptions SocketOption;
        /// <summary>
        /// Correo del remitente
        /// </summary>
        private string SenderEmail;
        /// <summary>
        /// Contraseña del remitente
        /// </summary>
        private string Password;
        /// <summary>
        /// Nombre del remitente
        /// </summary>
        private string SenderName;
        /// <summary>
        /// Establece las variables de configuracion necesarias para el envio del correo
        /// </summary>
        /// <param name="mailServer">Servidor de correos</param>
        /// <param name="port">Puerto del servidor de correos</param>
        /// <param name="socketOption">Tipo de socket</param>
        /// <param name="senderEmail">Correo del remitente</param>
        /// <param name="password">Contraseña del remitente</param>
        /// <param name="senderName">Nombre del remitente</param>
        public void Configure(string mailServer, int port, SecureSocketOptions socketOption, string senderEmail, string password, string senderName)
        {
            Port = port;
            SocketOption = socketOption;
            _ = mailServer ?? throw new ArgumentNullException(nameof(mailServer));
            MailServer = mailServer;
            _ = senderEmail ?? throw new ArgumentNullException(nameof(senderEmail));
            SenderEmail = senderEmail;
            _ = password ?? throw new ArgumentNullException(nameof(password));
            Password = password;
            _ = senderName ?? throw new ArgumentNullException(nameof(senderName));
            SenderName = senderName;
        }
        /// <summary>
        /// Envio del correo eletronico
        /// </summary>
        /// <param name="subject">Asunto</param>
        /// <param name="body">Cuerpo del mensaje</param>
        /// <param name="to">Conjunto de destinatario</param>
        /// <param name="cc">Conjunto de destinatarios a quienes debe copiarse el mensaje</param>
        /// <param name="bcc">Conjunto de destinatarios ocultos</param>
        /// <param name="attachments">Lista de archivos adjuntos</param>
        /// <param name="ct">Token de cancelacion</param>
        /// <returns></returns>
        public async Task SendAsync(string? subject, string body, string[] to, string[] cc, string[] bcc, Models.Attachment[]? attachments, CancellationToken ct = default)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(SenderName, SenderEmail));
            message.Subject = subject;
            if (to == null || !to.Where(w => !string.IsNullOrEmpty(w)).Any())
                throw new ArgumentException("At least one addressee is required for the sending of the notification");

            foreach (var receiver in to.Where(w => !string.IsNullOrEmpty(w)))
                message.To.Add(new MailboxAddress(receiver, receiver));

            var builder = new BodyBuilder { HtmlBody = body };
            if (cc != null)
                foreach (var receiver in cc.Where(w => !string.IsNullOrEmpty(w)))
                    message.Cc.Add(new MailboxAddress(receiver, receiver));
            if (bcc != null)
                foreach (var receiver in bcc.Where(w => !string.IsNullOrEmpty(w)))
                    message.Bcc.Add(new MailboxAddress(receiver, receiver));
            if (attachments != null)
                foreach (var file in attachments)
                    using (var att = new MemoryStream(Convert.FromBase64String(file.Hash)))
                        builder.Attachments.Add(file.FileName, att, ContentType.Parse(file.ContentType), ct);

            message.Body = builder.ToMessageBody();
            using var client = new SmtpClient();
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            await client.ConnectAsync(MailServer, Port, SocketOption, ct);
            await client.AuthenticateAsync(SenderEmail, Password, ct);
            await client.SendAsync(message, ct);
            await client.DisconnectAsync(true, ct);
        }
    }
}
