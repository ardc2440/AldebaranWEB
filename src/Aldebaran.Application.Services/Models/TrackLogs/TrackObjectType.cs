namespace Aldebaran.Application.Services.Models.TrackLogs
{
    /// <summary>Tipo de objeto de negocio por el que se puede buscar (artículo, pedido, cliente, ...).</summary>
    /// <param name="Hint">Qué puede escribir el usuario para encontrarlo.</param>
    public sealed record TrackObjectType(byte ObjectType, string Name, string Hint);
}
