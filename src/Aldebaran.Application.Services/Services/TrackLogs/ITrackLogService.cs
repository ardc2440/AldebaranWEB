using Aldebaran.Application.Services.Models.AlarmLogs;
using Aldebaran.Application.Services.Models.TrackLogs;

namespace Aldebaran.Application.Services.TrackLogs
{
    /// <summary>Casos de uso de la consulta de Track (trazabilidad de cambios por entidad u objeto de negocio).</summary>
    public interface ITrackLogService
    {
        /// <summary>Entidades trazadas que se pueden consultar.</summary>
        Task<IReadOnlyList<TrackEntity>> GetEntitiesAsync(CancellationToken ct = default);

        /// <summary>Tipos de objeto de negocio por los que se puede buscar.</summary>
        Task<IReadOnlyList<TrackObjectType>> GetObjectTypesAsync(CancellationToken ct = default);

        /// <summary>Objetos de un tipo que coinciden con lo que escribió el usuario (mínimo 2 caracteres).</summary>
        Task<IReadOnlyList<TrackObjectOption>> SearchObjectsAsync(byte objectType, string? text, CancellationToken ct = default);

        /// <summary>Movimientos según los filtros.</summary>
        Task<PagedResult<TrackEntry>> GetEntriesAsync(TrackLogFilter filter, int skip, int take, CancellationToken ct = default);

        /// <summary>Detalle campo / anterior / nuevo de un movimiento, con la descripción de los Id conocidos (null si no existe).</summary>
        Task<TrackDetail?> GetDetailAsync(long id, string entityName, CancellationToken ct = default);
    }
}
