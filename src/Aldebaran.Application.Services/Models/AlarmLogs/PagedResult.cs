namespace Aldebaran.Application.Services.Models.AlarmLogs
{
    /// <summary>Página de resultados con el total de filas de la consulta.</summary>
    public sealed record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount)
    {
        public static PagedResult<T> Empty { get; } = new(Array.Empty<T>(), 0);
    }
}
