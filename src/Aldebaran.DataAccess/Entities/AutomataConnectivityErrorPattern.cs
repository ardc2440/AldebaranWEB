using Aldebaran.DataAccess.Core;
namespace Aldebaran.DataAccess.Entities
{
    public class AutomataConnectivityErrorPattern : ITrackeable
    {
        public int Id { get; set; }
        public string Pattern { get; set; } = null!;
        public string Target { get; set; } = null!;
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? Notes { get; set; }
    }
}
