namespace Aldebaran.Application.Services.Models.AlarmLogs
{
    /// <summary>Filtros de la consulta de refrescos e inactivaciones de alarmas de inventario.</summary>
    /// <param name="DateFrom">Fecha inicial del evento (obligatoria).</param>
    /// <param name="DateTo">Fecha final del evento (obligatoria).</param>
    /// <param name="AlarmTypeCode">Tipo de alarma (null = todos).</param>
    /// <param name="EventType">Tipo de evento (null = todos).</param>
    /// <param name="SearchKey">Texto a buscar en la referencia o el origen.</param>
    public sealed record InventoryAlarmLogFilter(DateTime DateFrom, DateTime DateTo, string? AlarmTypeCode = null, InventoryAlarmEventType? EventType = null, string? SearchKey = null);
}
