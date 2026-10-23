using Aldebaran.Application.Services.Models.AlarmLogs;
using Aldebaran.DataAccess.Infraestructure.Repository;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services.AlarmLogs
{
    /// <summary>
    /// Orquestador de la consulta de log de alarmas: valida los filtros, consulta el repositorio
    /// y entrega los modelos del caso de uso (no las entidades de datos).
    /// </summary>
    public class AlarmLogService : IAlarmLogService
    {
        private const int MaxTake = 500;
        private static readonly HashSet<string> InventoryLogTypeCodes = new() { "MINIMUM_QUANTITY", "MINIMUM_LOCAL_WAREHOUSE", "OUT_OF_STOCK" };
        private readonly IAlarmLogRepository _repository;

        public AlarmLogService(IAlarmLogRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<IReadOnlyList<AlarmLogType>> GetTypesAsync(AlarmLogContext context, IEnumerable<string> userRoles, CancellationToken ct = default)
        {
            var roles = JoinRoles(userRoles);
            if (roles.Length == 0)
                return Array.Empty<AlarmLogType>();

            var rows = await _repository.GetTypesAsync(ToContextCode(context), roles, ct);
            return rows.Select(r => new AlarmLogType(r.Code, r.Name)).ToList();
        }

        public async Task<PagedResult<AlarmLogItem>> GetAlarmsAsync(string typeCode, DateTime dateFrom, DateTime dateTo, string? searchKey, int skip, int take, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(typeCode))
                throw new ArgumentException("El tipo de alarma es obligatorio.", nameof(typeCode));
            ValidateDateRange(dateFrom, dateTo);

            var rows = (await _repository.GetAlarmsAsync(typeCode, dateFrom, dateTo, NormalizeSearch(searchKey), NormalizeSkip(skip), NormalizeTake(take), ct)).ToList();
            return ToPage(rows, r => r.TotalRows, ToModel);
        }

        public async Task<IReadOnlyList<AlarmVisualization>> GetVisualizationsAsync(string typeCode, string alarmKey, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(typeCode) || string.IsNullOrWhiteSpace(alarmKey))
                return Array.Empty<AlarmVisualization>();

            var rows = await _repository.GetVisualizationsAsync(typeCode, alarmKey, ct);
            return rows.Select(r => new AlarmVisualization(r.EmployeeName, r.AreaName, r.VisualizedDate)).ToList();
        }

        public async Task<IReadOnlyList<AlarmLogType>> GetInventoryLogTypesAsync(IEnumerable<string> userRoles, CancellationToken ct = default)
        {
            var types = await GetTypesAsync(AlarmLogContext.Inventory, userRoles, ct);
            return types.Where(t => InventoryLogTypeCodes.Contains(t.Code)).ToList();
        }

        public async Task<PagedResult<InventoryAlarmEvent>> GetInventoryAlarmEventsAsync(InventoryAlarmLogFilter filter, int skip, int take, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(filter);
            ValidateDateRange(filter.DateFrom, filter.DateTo);

            var rows = (await _repository.GetInventoryAlarmLogAsync(filter.DateFrom, filter.DateTo,
                string.IsNullOrWhiteSpace(filter.AlarmTypeCode) ? null : filter.AlarmTypeCode,
                ToEventTypeCode(filter.EventType), NormalizeSearch(filter.SearchKey),
                NormalizeSkip(skip), NormalizeTake(take), ct)).ToList();
            return ToPage(rows, r => r.TotalRows, ToModel);
        }

        /* ---------- Reglas del caso de uso ---------- */

        private static void ValidateDateRange(DateTime dateFrom, DateTime dateTo)
        {
            if (dateFrom == default || dateTo == default)
                throw new ArgumentException("El rango de fechas es obligatorio.");
            if (dateFrom.Date > dateTo.Date)
                throw new ArgumentException("La fecha inicial no puede ser mayor que la final.");
        }

        private static string JoinRoles(IEnumerable<string> userRoles) =>
            string.Join(",", (userRoles ?? Enumerable.Empty<string>()).Where(r => !string.IsNullOrWhiteSpace(r)).Select(r => r.Trim()).Distinct());

        private static string? NormalizeSearch(string? searchKey) => string.IsNullOrWhiteSpace(searchKey) ? null : searchKey.Trim();

        private static int NormalizeSkip(int skip) => Math.Max(skip, 0);

        private static int NormalizeTake(int take) => take <= 0 ? 20 : Math.Min(take, MaxTake);

        private static char ToContextCode(AlarmLogContext context) => context switch
        {
            AlarmLogContext.PurchaseOrders => 'O',
            AlarmLogContext.CustomerOrders => 'P',
            AlarmLogContext.CustomerReservations => 'R',
            AlarmLogContext.Inventory => 'I',
            _ => throw new ArgumentOutOfRangeException(nameof(context))
        };

        private static string? ToEventTypeCode(InventoryAlarmEventType? eventType) => eventType switch
        {
            InventoryAlarmEventType.Refresh => "R",
            InventoryAlarmEventType.Inactivation => "I",
            _ => null
        };

        private static InventoryAlarmEventType ToEventType(string code) =>
            code == "I" ? InventoryAlarmEventType.Inactivation : InventoryAlarmEventType.Refresh;

        /* ---------- Mapeos ---------- */

        private static PagedResult<TModel> ToPage<TRow, TModel>(IReadOnlyList<TRow> rows, Func<TRow, int> totalRows, Func<TRow, TModel> map) =>
            rows.Count == 0 ? PagedResult<TModel>.Empty : new PagedResult<TModel>(rows.Select(map).ToList(), totalRows(rows[0]));

        private static AlarmLogItem ToModel(Entities.AlarmLogEntry row) =>
            new(row.AlarmKey, row.AlarmDate, row.Document, row.Description, row.VisualizationCount);

        private static InventoryAlarmEvent ToModel(Entities.InventoryAlarmLogEntry row) =>
            new(row.InventoryAlarmLogId,
                row.EventDate,
                row.AlarmTypeCode,
                row.AlarmTypeName,
                row.AlarmId,
                row.ReferenceId,
                row.ReferenceDescription,
                ToEventType(row.EventType),
                row.PreviousQuantity,
                row.NewQuantity,
                row.PreviousMinimumQuantity,
                row.NewMinimumQuantity,
                row.PreviousCreationDate,
                row.NewCreationDate,
                row.InactivationDate,
                row.VisualizationCount,
                row.VisualizationsRemoved,
                row.Source);
    }
}
