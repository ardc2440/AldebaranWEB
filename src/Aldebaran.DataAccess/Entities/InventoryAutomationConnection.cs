using Aldebaran.DataAccess.Core;
using Aldebaran.DataAccess.Core.Atributes;
namespace Aldebaran.DataAccess.Entities
{
    public class InventoryAutomationConnection : ITrackeable
    {
        public int InventoryAutomationConnectionId { get; set; }
        public string ServerName { get; set; } = null!;
        public string? PortNumber { get; set; }
        public string DatabaseName { get; set; } = null!;
        public string UserId { get; set; } = null!;
        [TrackMasked]
        public string Password { get; set; } = null!;
        public bool Active { get; set; }
    }
}