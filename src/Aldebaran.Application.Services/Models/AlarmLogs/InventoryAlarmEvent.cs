namespace Aldebaran.Application.Services.Models.AlarmLogs
{
    /// <summary>Refresco o inactivación de una alarma de inventario, con el estado anterior y el nuevo.</summary>
    /// <param name="VisualizationCount">Visualizaciones que tenía la alarma al momento del evento.</param>
    /// <param name="VisualizationsRemoved">El evento borró esas visualizaciones (la alarma volvió a salirle a todos).</param>
    /// <param name="Source">Origen del evento (movimiento, proceso programado, etc.).</param>
    public sealed record InventoryAlarmEvent(
        int LogId,
        DateTime EventDate,
        string AlarmTypeCode,
        string AlarmTypeName,
        int AlarmId,
        int ReferenceId,
        string ReferenceDescription,
        InventoryAlarmEventType EventType,
        int? PreviousQuantity,
        int? NewQuantity,
        int? PreviousMinimumQuantity,
        int? NewMinimumQuantity,
        DateTime? PreviousCreationDate,
        DateTime? NewCreationDate,
        DateTime? InactivationDate,
        int VisualizationCount,
        bool VisualizationsRemoved,
        string Source);
}
