using Aldebaran.Application.Services.Models.AlarmLogs;
using Aldebaran.Application.Services.Models.OrderDocumentLogs;

namespace Aldebaran.Application.Services.OrderDocumentLogs
{
    /// <summary>Casos de uso de los logs de modificación y cancelación de despachos y traslados a proceso.</summary>
    public interface IOrderDocumentLogService
    {
        Task<PagedResult<OrderDocumentLogItem>> GetLogAsync(OrderDocumentLogType logType, DateTime dateFrom, DateTime dateTo, string? searchKey, int skip, int take, CancellationToken ct = default);
    }
}
