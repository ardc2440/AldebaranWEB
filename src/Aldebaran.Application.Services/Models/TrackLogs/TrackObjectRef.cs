namespace Aldebaran.Application.Services.Models.TrackLogs
{
    /// <summary>Objeto de negocio elegido para filtrar el Track (tipo + Id).</summary>
    public sealed record TrackObjectRef(byte ObjectType, int ObjectId);
}
