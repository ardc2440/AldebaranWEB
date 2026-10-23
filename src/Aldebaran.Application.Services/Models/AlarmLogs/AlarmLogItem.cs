namespace Aldebaran.Application.Services.Models.AlarmLogs
{
    /// <summary>Alarma activa con la cantidad de usuarios que ya la vieron.</summary>
    /// <param name="AlarmKey">Llave de la alarma dentro de su tipo (para consultar sus visualizaciones).</param>
    /// <param name="AlarmDate">Fecha en la que la alarma se hizo visible en el tablero.</param>
    /// <param name="Document">Documento o referencia de la alarma.</param>
    /// <param name="Description">Descripción de la alarma.</param>
    /// <param name="VisualizationCount">Visualizaciones vigentes.</param>
    public sealed record AlarmLogItem(string AlarmKey, DateTime AlarmDate, string? Document, string? Description, int VisualizationCount);
}
