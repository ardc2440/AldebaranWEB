using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    /// <summary>Consultas de solo lectura del log de alarmas (visualizaciones y log de inventario).</summary>
    public interface IAlarmLogRepository
    {
        /// <summary>Tipos de alarma del contexto que los roles indicados pueden consultar (roles separados por coma).</summary>
        Task<IEnumerable<AlarmLogType>> GetTypesAsync(char context, string roles, CancellationToken ct = default);

        /// <summary>Alarmas activas del tipo, visibles entre las fechas (incluye TotalRows para paginar).</summary>
        Task<IEnumerable<AlarmLogEntry>> GetAlarmsAsync(string typeCode, DateTime dateFrom, DateTime dateTo, string? searchKey, int? employeeId, int skip, int take, CancellationToken ct = default);

        /// <summary>Visualizaciones vigentes de una alarma.</summary>
        Task<IEnumerable<AlarmLogVisualization>> GetVisualizationsAsync(string typeCode, string alarmKey, CancellationToken ct = default);

        /// <summary>Empleados que visualizaron alarmas activas del tipo (SP_GET_ALARM_LOG_VISUALIZERS).</summary>
        Task<IEnumerable<AlarmLogVisualizer>> GetVisualizersAsync(string typeCode, CancellationToken ct = default);

        /// <summary>Refrescos e inactivaciones de alarmas de inventario entre las fechas (incluye TotalRows para paginar).</summary>
        Task<IEnumerable<InventoryAlarmLogEntry>> GetInventoryAlarmLogAsync(DateTime dateFrom, DateTime dateTo, string? alarmTypeCode, string? eventType, string? searchKey, int skip, int take, CancellationToken ct = default);
    }
}
