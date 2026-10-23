namespace Aldebaran.DataAccess.Entities
{
    /// <summary>
    /// Fila de SP_GET_ALARM_LOG_TYPES: tipo de alarma que el usuario puede consultar en un contexto.
    /// Entidad keyless (solo lectura).
    /// </summary>
    public class AlarmLogType
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
