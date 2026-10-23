using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    /// <summary>Consulta de solo lectura del log general de la aplicación (log.Logs).</summary>
    public interface IGeneralLogRepository
    {
        Task<IEnumerable<GeneralLogGroupEntry>> GetGroupsAsync(DateTime dateFrom, DateTime dateTo, string? level, string? source, string? searchKey, int skip, int take, CancellationToken ct = default);
        Task<IEnumerable<GeneralLogOccurrenceEntry>> GetOccurrencesAsync(string? groupKey, DateTime dateFrom, DateTime dateTo, string? level, string? source, int skip, int take, CancellationToken ct = default);
        Task<GeneralLogDetailEntry?> GetDetailAsync(long logId, CancellationToken ct = default);
        Task<IEnumerable<string>> GetSourcesAsync(CancellationToken ct = default);
    }
}
