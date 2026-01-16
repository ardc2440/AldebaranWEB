namespace Aldebaran.Application.Services.Models
{
    public class InventoryAutomationConnection
    {
        public int InventoryAutomationConnectionId { get; set; }
        public string ServerName { get; set; } = null!;
        public string? PortNumber { get; set; }
        public string DatabaseName { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool Active { get; set; }
    }
}