using Aldebaran.Application.Services.Models.AlarmLogs;

namespace Aldebaran.Application.Services.AlarmLogs
{
    /// <summary>Casos de uso de la consulta de log de alarmas.</summary>
    public interface IAlarmLogService
    {
        /// <summary>Tipos de alarma del contexto que el usuario puede consultar según sus roles.</summary>
        Task<IReadOnlyList<AlarmLogType>> GetTypesAsync(AlarmLogContext context, IEnumerable<string> userRoles, CancellationToken ct = default);

        /// <summary>Alarmas activas del tipo, visibles en el rango de fechas, con su cantidad de visualizaciones.</summary>
        Task<PagedResult<AlarmLogItem>> GetAlarmsAsync(string typeCode, DateTime dateFrom, DateTime dateTo, string? searchKey, int skip, int take, CancellationToken ct = default);

        /// <summary>Visualizaciones vigentes de una alarma.</summary>
        Task<IReadOnlyList<AlarmVisualization>> GetVisualizationsAsync(string typeCode, string alarmKey, CancellationToken ct = default);

        /// <summary>Tipos de alarma de inventario que registran refrescos e inactivaciones y que el usuario puede consultar.</summary>
        Task<IReadOnlyList<AlarmLogType>> GetInventoryLogTypesAsync(IEnumerable<string> userRoles, CancellationToken ct = default);

        /// <summary>Refrescos e inactivaciones de alarmas de inventario.</summary>
        Task<PagedResult<InventoryAlarmEvent>> GetInventoryAlarmEventsAsync(InventoryAlarmLogFilter filter, int skip, int take, CancellationToken ct = default);
    }
}
