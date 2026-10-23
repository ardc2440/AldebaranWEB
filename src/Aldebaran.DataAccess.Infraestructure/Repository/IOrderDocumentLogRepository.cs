using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    /// <summary>Consulta de solo lectura de los logs de modificación/cancelación de despachos y traslados a proceso.</summary>
    public interface IOrderDocumentLogRepository
    {
        Task<IEnumerable<OrderDocumentLogEntry>> GetLogAsync(string logType, DateTime dateFrom, DateTime dateTo, string? searchKey, int skip, int take, CancellationToken ct = default);
    }
}
