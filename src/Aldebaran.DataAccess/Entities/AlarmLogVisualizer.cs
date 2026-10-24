namespace Aldebaran.DataAccess.Entities
{
    /// <summary>
    /// Fila de SP_GET_ALARM_LOG_VISUALIZERS: empleado que visualizó alarmas activas del tipo.
    /// Entidad keyless (solo lectura).
    /// </summary>
    public class AlarmLogVisualizer
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
    }
}
