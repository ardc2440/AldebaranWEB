using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    /// <summary>Consultas de solo lectura de log.track y de su índice de objetos de negocio.</summary>
    public interface ITrackLogRepository
    {
        /// <summary>Entidades trazadas que se pueden consultar.</summary>
        Task<IEnumerable<TrackEntityName>> GetEntitiesAsync(CancellationToken ct = default);

        /// <summary>Tipos de objeto de negocio por los que se puede buscar.</summary>
        Task<IEnumerable<TrackObjectType>> GetObjectTypesAsync(CancellationToken ct = default);

        /// <summary>Objetos de un tipo que coinciden con el texto (lo que conoce el usuario).</summary>
        Task<IEnumerable<TrackObjectMatch>> SearchObjectsAsync(byte objectType, string text, CancellationToken ct = default);

        /// <summary>Movimientos por entidad y/o por objeto entre las fechas (incluye TotalRows para paginar).</summary>
        Task<IEnumerable<TrackLogEntry>> GetLogAsync(string? entityName, DateTime dateFrom, DateTime dateTo, string? action, string? modifierName, byte? objectType, int? objectId, int skip, int take, CancellationToken ct = default);

        /// <summary>Datos del movimiento y del anterior de la misma entidad y llave (null si no existe).</summary>
        Task<TrackLogDetail?> GetDetailAsync(long id, CancellationToken ct = default);

        /// <summary>Nombres para el usuario de los campos de una entidad (los no registrados no vienen).</summary>
        Task<IEnumerable<TrackFieldName>> GetFieldNamesAsync(string entityName, CancellationToken ct = default);

        /// <summary>Descripción de los Id de objetos que aparecen en el movimiento y en el anterior.</summary>
        Task<IEnumerable<TrackObjectDescription>> GetObjectDescriptionsAsync(long id, CancellationToken ct = default);
    }
}
