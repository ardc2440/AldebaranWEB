namespace Aldebaran.Application.Services.InventoryMinimumAlerts.Models
{
    public class InventoryMinimumAlertSettings
    {
        public bool Enabled { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public required string NotificationSettings { get; set; }
        public required string NotificationSubject { get; set; }
        public required string MarkAsReadUrl { get; set; }        
        public List<DayOfWeek> DaysOfWeek { get; set; } = new();
        public List<TimeSpan> ExecutionHours { get; set; } = new();
        /// <summary>Lado mayor (px) de las miniaturas incrustadas en el Excel del correo.</summary>
        public int ThumbnailMaxSize { get; set; } = 200;
        /// <summary>Tamaño máximo del Excel adjunto (MB). Si se supera, se envía sin imágenes (límite Gmail 25 MB con base64 +33 %).</summary>
        public int MaxAttachmentMegabytes { get; set; } = 15;
    }
}
