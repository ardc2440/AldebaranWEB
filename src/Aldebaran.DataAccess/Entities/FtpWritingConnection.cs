using Aldebaran.DataAccess.Core;
using Aldebaran.DataAccess.Core.Atributes;
namespace Aldebaran.DataAccess.Entities
{
    public class FtpWritingConnection : ITrackeable
    {
        public int FtpWritingConnectionId { get; set; }
        public string HostName { get; set; } = null!;
        public string? PortNumber { get; set; }
        public string UserName { get; set; } = null!;
        [TrackMasked]
        public string Password { get; set; } = null!;
        public bool? RewriteFile { get; set; }
        public bool Active { get; set; }
    }
}