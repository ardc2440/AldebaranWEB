namespace Aldebaran.Application.Services.Models
{
    public class AutomataConnectivityErrorPattern
    {
        public int Id { get; set; }
        public string Pattern { get; set; } = null!;
        public string Target { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? Notes { get; set; }
    }
}
