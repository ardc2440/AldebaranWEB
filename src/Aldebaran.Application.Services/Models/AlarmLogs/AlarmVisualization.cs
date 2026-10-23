namespace Aldebaran.Application.Services.Models.AlarmLogs
{
    /// <summary>Usuario que vio la alarma y cuándo.</summary>
    public sealed record AlarmVisualization(string EmployeeName, string? AreaName, DateTime VisualizedDate);
}
