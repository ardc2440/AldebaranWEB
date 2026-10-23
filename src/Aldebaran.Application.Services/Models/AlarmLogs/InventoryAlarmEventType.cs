namespace Aldebaran.Application.Services.Models.AlarmLogs
{
    /// <summary>Evento registrado en el log de alarmas de inventario.</summary>
    public enum InventoryAlarmEventType
    {
        /// <summary>La condición se volvió a dar con la alarma activa: estado más reciente (y visualizaciones borradas, salvo el proceso programado).</summary>
        Refresh,
        /// <summary>La alarma murió.</summary>
        Inactivation
    }
}
