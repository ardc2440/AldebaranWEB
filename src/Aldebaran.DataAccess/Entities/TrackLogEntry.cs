namespace Aldebaran.DataAccess.Entities
{
    /// <summary>Fila de SP_GET_TRACK_LOG: movimiento de una entidad en log.track. Entidad keyless (solo lectura).</summary>
    public class TrackLogEntry
    {
        public long Id { get; set; }
        public string EntityName { get; set; } = string.Empty;
        public string EntityDisplayName { get; set; } = string.Empty;
        public string? EntityKey { get; set; }
        public string? RecordDescription { get; set; }
        public string Action { get; set; } = string.Empty;
        public DateTime ModifiedDate { get; set; }
        public string ModifierName { get; set; } = string.Empty;
        public int TotalRows { get; set; }
    }
}
