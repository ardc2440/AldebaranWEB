using Aldebaran.Application.Services.Models.AlarmLogs;
using Aldebaran.Application.Services.Models.PurchaseOrderLogs;
using Aldebaran.DataAccess.Infraestructure.Repository;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services.PurchaseOrderLogs
{
    /// <summary>Orquestador de los logs de OC: valida filtros y entrega el modelo del caso de uso.</summary>
    public class PurchaseOrderLogService : IPurchaseOrderLogService
    {
        private const int MaxTake = 500;
        private readonly IPurchaseOrderLogRepository _repository;

        public PurchaseOrderLogService(IPurchaseOrderLogRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<PagedResult<PurchaseOrderAdjustmentRequestItem>> GetAdjustmentRequestsAsync(DateTime dateFrom, DateTime dateTo, string? searchKey, int skip, int take, CancellationToken ct = default)
        {
            ValidateRange(dateFrom, dateTo);
            var rows = (await _repository.GetAdjustmentRequestsAsync(dateFrom, dateTo, Normalize(searchKey), Math.Max(skip, 0), Take(take), ct)).ToList();
            return rows.Count == 0
                ? PagedResult<PurchaseOrderAdjustmentRequestItem>.Empty
                : new PagedResult<PurchaseOrderAdjustmentRequestItem>(rows.Select(ToModel).ToList(), rows[0].TotalRows);
        }

        public async Task<PagedResult<ApprovalRangeLogItem>> GetApprovalRangeChangesAsync(DateTime dateFrom, DateTime dateTo, string? searchKey, int skip, int take, CancellationToken ct = default)
        {
            ValidateRange(dateFrom, dateTo);
            var rows = (await _repository.GetApprovalRangeChangesAsync(dateFrom, dateTo, Normalize(searchKey), Math.Max(skip, 0), Take(take), ct)).ToList();
            return rows.Count == 0
                ? PagedResult<ApprovalRangeLogItem>.Empty
                : new PagedResult<ApprovalRangeLogItem>(rows.Select(ToModel).ToList(), rows[0].TotalRows);
        }

        private static void ValidateRange(DateTime dateFrom, DateTime dateTo)
        {
            if (dateFrom == default || dateTo == default)
                throw new ArgumentException("El rango de fechas es obligatorio.");
            if (dateFrom.Date > dateTo.Date)
                throw new ArgumentException("La fecha inicial no puede ser mayor que la final.");
        }

        private static string? Normalize(string? searchKey) => string.IsNullOrWhiteSpace(searchKey) ? null : searchKey.Trim();

        private static int Take(int take) => take <= 0 ? 20 : Math.Min(take, MaxTake);

        private static PurchaseOrderAdjustmentRequestItem ToModel(Entities.PurchaseOrderAdjustmentRequestEntry row) =>
            new(row.RequestId, row.RequestDate, row.PurchaseOrderId, row.OrderNumber, row.ProviderName, row.RequestedBy,
                row.Result, row.ResolutionDate, row.ResolvedBy, row.Reason);

        private static ApprovalRangeLogItem ToModel(Entities.ApprovalRangeLogEntry row) =>
            new(row.LogId, row.EventDate,
                new ApprovalRangeValues(row.PreviousQuantityFrom, row.PreviousQuantityTo, row.PreviousPercent, row.PreviousIsActive),
                new ApprovalRangeValues(row.QuantityFrom, row.QuantityTo, row.Percent, row.IsActive),
                row.ChangeReason, row.EmployeeName);
    }
}
