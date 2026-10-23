using Aldebaran.Application.Services.Models.AlarmLogs;
using Aldebaran.Application.Services.Models.TrackLogs;
using Aldebaran.DataAccess.Infraestructure.Repository;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services.TrackLogs
{
    /// <summary>
    /// Orquestador de la consulta de Track: valida filtros, consulta el repositorio y arma el detalle
    /// campo / anterior / nuevo comparando el movimiento con el anterior del mismo registro.
    /// </summary>
    public class TrackLogService : ITrackLogService
    {
        private const int MaxTake = 500;
        private const int MinSearchLength = 2;
        private readonly ITrackLogRepository _repository;

        public TrackLogService(ITrackLogRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<IReadOnlyList<TrackEntity>> GetEntitiesAsync(CancellationToken ct = default)
        {
            var rows = await _repository.GetEntitiesAsync(ct);
            return rows.Select(r => new TrackEntity(r.EntityName, r.DisplayName)).ToList();
        }

        public async Task<IReadOnlyList<TrackObjectType>> GetObjectTypesAsync(CancellationToken ct = default)
        {
            var rows = await _repository.GetObjectTypesAsync(ct);
            return rows.Select(r => new TrackObjectType(r.ObjectType, r.Name, r.Hint)).ToList();
        }

        public async Task<IReadOnlyList<TrackObjectOption>> SearchObjectsAsync(byte objectType, string? text, CancellationToken ct = default)
        {
            var search = Normalize(text);
            if (objectType == 0 || search == null || search.Length < MinSearchLength)
                return Array.Empty<TrackObjectOption>();

            var rows = await _repository.SearchObjectsAsync(objectType, search, ct);
            return rows.Select(r => new TrackObjectOption(r.ObjectId, r.Description)).ToList();
        }

        public async Task<PagedResult<TrackEntry>> GetEntriesAsync(TrackLogFilter filter, int skip, int take, CancellationToken ct = default)
        {
            ValidateFilter(filter);

            var rows = (await _repository.GetLogAsync(Normalize(filter.EntityName), filter.DateFrom, filter.DateTo,
                TrackActionCodes.ToCode(filter.Action), Normalize(filter.ModifierName),
                filter.Object?.ObjectType, filter.Object?.ObjectId,
                Math.Max(skip, 0), take <= 0 ? 20 : Math.Min(take, MaxTake), ct)).ToList();

            return rows.Count == 0
                ? PagedResult<TrackEntry>.Empty
                : new PagedResult<TrackEntry>(rows.Select(ToModel).ToList(), rows[0].TotalRows);
        }

        public async Task<TrackDetail?> GetDetailAsync(long id, CancellationToken ct = default)
        {
            if (id <= 0)
                return null;

            var row = await _repository.GetDetailAsync(id, ct);
            if (row == null)
                return null;

            var descriptions = await _repository.GetObjectDescriptionsAsync(id, ct);
            return ToDetail(row, TrackObjectLabels.From(descriptions));
        }

        /* ---------- Reglas del caso de uso ---------- */

        private static void ValidateFilter(TrackLogFilter filter)
        {
            ArgumentNullException.ThrowIfNull(filter);
            if (string.IsNullOrWhiteSpace(filter.EntityName) && filter.Object == null)
                throw new ArgumentException("Seleccione la entidad o el objeto a consultar.", nameof(filter));
            if (filter.DateFrom == default || filter.DateTo == default)
                throw new ArgumentException("El rango de fechas es obligatorio.", nameof(filter));
            if (filter.DateFrom.Date > filter.DateTo.Date)
                throw new ArgumentException("La fecha inicial no puede ser mayor que la final.", nameof(filter));
        }

        private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        /* ---------- Mapeos ---------- */

        private static TrackEntry ToModel(Entities.TrackLogEntry row) =>
            new(row.Id, row.EntityName, row.EntityDisplayName, TrackDataParser.FormatKey(row.EntityKey),
                TrackActionCodes.FromCode(row.Action), row.ModifiedDate, row.ModifierName);

        private static TrackDetail ToDetail(Entities.TrackLogDetail row, TrackObjectLabels labels)
        {
            var action = TrackActionCodes.FromCode(row.Action);
            var current = TrackDataParser.Parse(row.DataLog);
            var previous = row.PreviousDataLog == null ? null : TrackDataParser.Parse(row.PreviousDataLog);

            var changes = TrackDiff.Build(action, current, previous).Select(labels.Apply).ToList();
            return new TrackDetail(row.Id, action, previous != null, changes);
        }
    }
}
