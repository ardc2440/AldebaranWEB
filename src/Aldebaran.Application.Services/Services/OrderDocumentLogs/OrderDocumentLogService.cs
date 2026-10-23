using Aldebaran.Application.Services.Models.AlarmLogs;
using Aldebaran.Application.Services.Models.OrderDocumentLogs;
using Aldebaran.DataAccess.Infraestructure.Repository;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services.OrderDocumentLogs
{
    /// <summary>Orquestador de los logs de despachos y traslados a proceso: valida filtros y entrega el modelo del caso de uso.</summary>
    public class OrderDocumentLogService : IOrderDocumentLogService
    {
        private const int MaxTake = 500;
        private readonly IOrderDocumentLogRepository _repository;

        public OrderDocumentLogService(IOrderDocumentLogRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<PagedResult<OrderDocumentLogItem>> GetLogAsync(OrderDocumentLogType logType, DateTime dateFrom, DateTime dateTo, string? searchKey, int skip, int take, CancellationToken ct = default)
        {
            if (dateFrom == default || dateTo == default)
                throw new ArgumentException("El rango de fechas es obligatorio.");
            if (dateFrom.Date > dateTo.Date)
                throw new ArgumentException("La fecha inicial no puede ser mayor que la final.");

            var rows = (await _repository.GetLogAsync(ToCode(logType), dateFrom, dateTo,
                string.IsNullOrWhiteSpace(searchKey) ? null : searchKey.Trim(),
                Math.Max(skip, 0), take <= 0 ? 20 : Math.Min(take, MaxTake), ct)).ToList();

            return rows.Count == 0
                ? PagedResult<OrderDocumentLogItem>.Empty
                : new PagedResult<OrderDocumentLogItem>(rows.Select(ToModel).ToList(), rows[0].TotalRows);
        }

        private static string ToCode(OrderDocumentLogType logType) => logType switch
        {
            OrderDocumentLogType.ShipmentModification => "SHIPMENT_MODIFICATION",
            OrderDocumentLogType.ShipmentCancellation => "SHIPMENT_CANCELLATION",
            OrderDocumentLogType.InProcessModification => "IN_PROCESS_MODIFICATION",
            OrderDocumentLogType.InProcessCancellation => "IN_PROCESS_CANCELLATION",
            _ => throw new ArgumentOutOfRangeException(nameof(logType))
        };

        private static OrderDocumentLogItem ToModel(Entities.OrderDocumentLogEntry row) =>
            new(row.LogKey, row.EventDate, row.DocumentId, row.DocumentDate, row.CustomerOrderId,
                row.OrderNumber, row.CustomerName, row.Reason, row.EmployeeName);
    }
}
