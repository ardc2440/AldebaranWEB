namespace Aldebaran.DataAccess.Entities
{
    /// <summary>
    /// Fila de SP_GET_INVENTORY_ALARM_LOG: refresco (R) o inactivación (I) de una alarma de inventario,
    /// con el estado anterior y el nuevo. Entidad keyless (solo lectura).
    /// </summary>
    public class InventoryAlarmLogEntry
    {
        public int InventoryAlarmLogId { get; set; }
        public DateTime EventDate { get; set; }
        public string AlarmTypeCode { get; set; } = string.Empty;
        public string AlarmTypeName { get; set; } = string.Empty;
        public int AlarmId { get; set; }
        public int ReferenceId { get; set; }
        public string ReferenceDescription { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public int? PreviousQuantity { get; set; }
        public int? NewQuantity { get; set; }
        public int? PreviousMinimumQuantity { get; set; }
        public int? NewMinimumQuantity { get; set; }
        public DateTime? PreviousCreationDate { get; set; }
        public DateTime? NewCreationDate { get; set; }
        public DateTime? InactivationDate { get; set; }
        public int VisualizationCount { get; set; }
        public bool VisualizationsRemoved { get; set; }
        public string Source { get; set; } = string.Empty;
        public int TotalRows { get; set; }
    }
}
