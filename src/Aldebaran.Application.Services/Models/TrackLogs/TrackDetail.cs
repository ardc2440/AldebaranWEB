namespace Aldebaran.Application.Services.Models.TrackLogs
{
    /// <summary>Detalle de un movimiento: campo / anterior / nuevo.</summary>
    /// <param name="HasPrevious">Existe un movimiento anterior del mismo registro con el cual comparar.</param>
    public sealed record TrackDetail(long Id, TrackAction Action, bool HasPrevious, IReadOnlyList<TrackChange> Changes);
}
