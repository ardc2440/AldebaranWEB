using Newtonsoft.Json;

namespace Aldebaran.DataAccess.Infraestructure.Models
{
    public class EmailNotificationProvider
    {
        [JsonProperty("mail_server")]
        public string MailServer { get; set; } = null!;
        [JsonProperty("port")]
        public int Port { get; set; }
        [JsonProperty("sender_name")]
        public string SenderName { get; set; } = null!;
        [JsonProperty("sender_email")]
        public string SenderEmail { get; set; } = null!;
        [JsonProperty("password")]
        public string Password { get; set; } = null!;
        [JsonProperty("secure_socket_option")]
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
