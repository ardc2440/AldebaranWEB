namespace Aldebaran.Application.Services.Models.TrackLogs
{
    /// <summary>Movimiento de una entidad.</summary>
    /// <param name="EntityDisplayName">Nombre de la entidad para mostrar.</param>
    /// <param name="EntityKey">Llave del registro afectado, legible (p. ej. "ReferenceId: 6161").</param>
    public sealed record TrackEntry(long Id, string EntityName, string EntityDisplayName, string EntityKey, TrackAction Action, DateTime ModifiedDate, string ModifierName);
}
