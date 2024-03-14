
using MailKit.Security;

namespace Aldebaran.Application.Services.Notificator.Services
{
    /// <summary>
    /// Contrato que provee mencanismos para el envio de correos
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Permite configuara el proveedor para el envio de correos
        /// </summary>
        /// <param name="mailServer">Servidor de correos</param>
        /// <param name="port">Puerto del servidor de correos</param>
        /// <param name="socketOption">Tipo de socket</param>
        /// <param name="senderEmail">Correo del remitente</param>
        /// <param name="password">Contraseña del remitente</param>
        /// <param name="senderName">Nombre del remitente</param>
        void Configure(string mailServer, int port, SecureSocketOptions socketOption, string senderEmail, string password, string senderName);
        /// <summary>
        /// Envia un correo eletronico
        /// </summary>
        /// <param name="subject">Asunto</param>
        /// <param name="body">Cuerpo del mensaje</param>
        /// <param name="to">Conjunto de destinatario</param>
        /// <param name="cc">Conjunto de destinatarios a quienes debe copiarse el mensaje</param>
        /// <param name="bcc">Conjunto de destinatarios ocultos</param>
        /// <param name="attachments">Lista de archivos adjuntos</param>
        /// <param name="ct">Token de cancelacion</param>
        /// <returns></returns>
        Task SendAsync(string? subject, string body, string[]? to, string[]? cc, string[]? bcc, Models.Attachment[]? attachments, CancellationToken ct = default);
    }
}
