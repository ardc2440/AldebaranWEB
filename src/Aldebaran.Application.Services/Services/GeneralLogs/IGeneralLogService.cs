using Aldebaran.Application.Services.Models.AlarmLogs;
using Aldebaran.Application.Services.Models.GeneralLogs;

namespace Aldebaran.Application.Services.GeneralLogs
{
    /// <summary>Casos de uso del log general: grupos → ocurrencias → detalle técnico.</summary>
    public interface IGeneralLogService
    {
        Task<PagedResult<GeneralLogGroup>> GetGroupsAsync(GeneralLogFilter filter, int skip, int take, CancellationToken ct = default);
        Task<PagedResult<GeneralLogOccurrence>> GetOccurrencesAsync(GeneralLogFilter filter, string? groupKey, int skip, int take, CancellationToken ct = default);
        Task<GeneralLogDetail?> GetDetailAsync(long logId, CancellationToken ct = default);
        Task<IReadOnlyList<string>> GetSourcesAsync(CancellationToken ct = default);
    }
}
