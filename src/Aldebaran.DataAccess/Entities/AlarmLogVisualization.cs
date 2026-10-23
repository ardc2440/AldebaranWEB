namespace Aldebaran.DataAccess.Entities
{
    /// <summary>
    /// Fila de SP_GET_ALARM_LOG_VISUALIZATIONS: quién vio la alarma y cuándo.
    /// Entidad keyless (solo lectura).
    /// </summary>
    public class AlarmLogVisualization
    {
        public string EmployeeName { get; set; } = string.Empty;
        public string? AreaName { get; set; }
        public DateTime VisualizedDate { get; set; }
    }
}
