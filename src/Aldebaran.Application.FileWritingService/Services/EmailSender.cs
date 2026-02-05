using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Aldebaran.Application.FileWritingService.Services
{
    internal class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;
        private readonly string _server;
        private readonly int _port;
        private readonly string _senderName;
        private readonly string _senderEmail;
        private readonly string _password;
        private readonly SecureSocketOptions _socketOption;

        public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _server = configuration["Mail:Server"] ?? throw new KeyNotFoundException("Mail:Server");
            _port = int.TryParse(configuration["Mail:Port"], out var p) ? p : throw new KeyNotFoundException("Mail:Port");
            _senderName = configuration["Mail:SenderName"] ?? throw new KeyNotFoundException("Mail:SenderName");
            _senderEmail = configuration["Mail:SenderEmail"] ?? throw new KeyNotFoundException("Mail:SenderEmail");
            _password = configuration["Mail:Password"] ?? string.Empty; // allow empty in dev
            var socket = configuration["Mail:SecureSocketOption"] ?? "StartTls";
            _socketOption = Enum.TryParse<SecureSocketOptions>(socket, out var so) ? so : SecureSocketOptions.StartTls;
        }

        public async Task SendAsync(string[] recipients, string subject, string body, CancellationToken ct = default)
        {
            if (recipients == null || recipients.Length == 0)
            {
                _logger.LogWarning("EmailSender.SendAsync called without recipients");
                return;
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_senderName, _senderEmail));
            message.Subject = subject;

            foreach (var r in recipients.Where(w => !string.IsNullOrWhiteSpace(w)))
                message.To.Add(new MailboxAddress(r, r));

            var builder = new BodyBuilder { HtmlBody = body };
            message.Body = builder.ToMessageBody();

            try
            {
                using var client = new SmtpClient();
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.ConnectAsync(_server, _port, _socketOption, ct);
                if (!string.IsNullOrEmpty(_password))
                    await client.AuthenticateAsync(_senderEmail, _password, ct);
                await client.SendAsync(message, ct);
                await client.DisconnectAsync(true, ct);
                _logger.LogInformation("Email sent to {RecipientsCount} recipients", recipients.Length);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email");
                throw;
            }
        }
    }
}
