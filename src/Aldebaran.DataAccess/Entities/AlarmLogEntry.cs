namespace Aldebaran.DataAccess.Entities
{
    /// <summary>
    /// Fila de SP_GET_ALARM_LOG: alarma activa de un tipo, con su cantidad de visualizaciones vigentes.
    /// Entidad keyless (solo lectura).
    /// </summary>
    public class AlarmLogEntry
    {
        public string AlarmKey { get; set; } = string.Empty;
        public DateTime AlarmDate { get; set; }
        public string? Document { get; set; }
        public string? Description { get; set; }
        public int VisualizationCount { get; set; }
        public int TotalRows { get; set; }
    }
}
