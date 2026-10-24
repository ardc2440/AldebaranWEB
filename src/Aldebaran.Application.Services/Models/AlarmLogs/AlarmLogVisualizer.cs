namespace Aldebaran.Application.Services.Models.AlarmLogs
{
    /// <summary>Empleado que visualizó alarmas del tipo (opción del filtro Empleado).</summary>
    public sealed record AlarmLogVisualizer(int EmployeeId, string EmployeeName);
}
