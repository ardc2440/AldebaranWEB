using Aldebaran.Application.Services.Models.AlarmLogs;
using Aldebaran.Application.Services.Models.GeneralLogs;
using Aldebaran.DataAccess.Infraestructure.Repository;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services.GeneralLogs
{
    /// <summary>Orquestador del log general: valida filtros y entrega el modelo del caso de uso.</summary>
    public class GeneralLogService : IGeneralLogService
    {
        private const int MaxTake = 500;
        private readonly IGeneralLogRepository _repository;

        public GeneralLogService(IGeneralLogRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<PagedResult<GeneralLogGroup>> GetGroupsAsync(GeneralLogFilter filter, int skip, int take, CancellationToken ct = default)
        {
            Validate(filter);
            var rows = (await _repository.GetGroupsAsync(filter.DateFrom, filter.DateTo, Normalize(filter.Level), Normalize(filter.Source),
                Normalize(filter.SearchKey), Math.Max(skip, 0), Take(take), ct)).ToList();
            return rows.Count == 0
                ? PagedResult<GeneralLogGroup>.Empty
                : new PagedResult<GeneralLogGroup>(rows.Select(ToModel).ToList(), rows[0].TotalRows);
        }

        public async Task<PagedResult<GeneralLogOccurrence>> GetOccurrencesAsync(GeneralLogFilter filter, string? groupKey, int skip, int take, CancellationToken ct = default)
        {
            Validate(filter);
            var rows = (await _repository.GetOccurrencesAsync(groupKey, filter.DateFrom, filter.DateTo, Normalize(filter.Level),
                Normalize(filter.Source), Math.Max(skip, 0), Take(take), ct)).ToList();
            return rows.Count == 0
                ? PagedResult<GeneralLogOccurrence>.Empty
                : new PagedResult<GeneralLogOccurrence>(rows.Select(ToModel).ToList(), rows[0].TotalRows);
        }

        public async Task<GeneralLogDetail?> GetDetailAsync(long logId, CancellationToken ct = default)
        {
            var row = await _repository.GetDetailAsync(logId, ct);
            return row == null ? null : new GeneralLogDetail(row.LogId, row.EventDate, Level(row.LogLevel), row.Source ?? string.Empty,
                row.Message ?? string.Empty, row.MessageTemplate ?? string.Empty, row.Exception, row.Properties);
        }

        public async Task<IReadOnlyList<string>> GetSourcesAsync(CancellationToken ct = default) =>
            (await _repository.GetSourcesAsync(ct)).ToList();

        private static void Validate(GeneralLogFilter filter)
        {
            ArgumentNullException.ThrowIfNull(filter);
            if (filter.DateFrom == default || filter.DateTo == default)
                throw new ArgumentException("El rango de fechas es obligatorio.");
            if (filter.DateFrom.Date > filter.DateTo.Date)
                throw new ArgumentException("La fecha inicial no puede ser mayor que la final.");
        }

        private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        private static int Take(int take) => take <= 0 ? 20 : Math.Min(take, MaxTake);

        private static string Level(string? level) => GeneralLogLevels.DisplayName(level);

        private static GeneralLogGroup ToModel(Entities.GeneralLogGroupEntry row) =>
            new(row.GroupKey, Level(row.LogLevel), row.Source ?? string.Empty, row.MessageTemplate ?? string.Empty,
                row.Occurrences, row.FirstDate, row.LastDate);

        private static GeneralLogOccurrence ToModel(Entities.GeneralLogOccurrenceEntry row) =>
            new(row.LogId, row.EventDate, Level(row.LogLevel), row.Source ?? string.Empty, row.Message ?? string.Empty, row.HasException);
    }
}
