namespace Aldebaran.Application.Services.Models
{
    public class FtpWritingConnection
    {
        public int FtpWritingConnectionId { get; set; }
        public string HostName { get; set; } = null!;
        public string? PortNumber { get; set; }
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool RewriteFile { get; set; }
        public bool Active { get; set; }
    }
}