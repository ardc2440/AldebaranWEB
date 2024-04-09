namespace Aldebaran.Application.Services.Models
{
    public class EmailNotificationProvider
    {
        public string MailServer { get; set; } = null!;
        public int Port { get; set; }
        public string SenderName { get; set; } = null!;
        public string SenderEmail { get; set; } = null!;
        public string Password { get; set; } = null!;
        public SecureSocketOptions SecureSocketOption { get; set; }
        public enum SecureSocketOptions
        {
            None,
            Auto,
            SslOnConnect,
            StartTls,
            StartTlsWhenAvailable,
        }
    }
}
