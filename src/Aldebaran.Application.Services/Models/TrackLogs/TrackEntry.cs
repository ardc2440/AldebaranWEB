namespace Aldebaran.Application.Services.Models.TrackLogs
{
    /// <summary>Movimiento de una entidad.</summary>
    /// <param name="EntityDisplayName">Nombre de la entidad para mostrar.</param>
    /// <param name="EntityKey">Registro afectado, legible para el usuario (p. ej. "Pedido 0000039469 - CLIENTE" o "Detalle de pedido 95364").</param>
    public sealed record TrackEntry(long Id, string EntityName, string EntityDisplayName, string EntityKey, TrackAction Action, DateTime ModifiedDate, string ModifierName);
}
