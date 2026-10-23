using Aldebaran.Application.Services.Models.AlarmLogs;
using Aldebaran.Application.Services.Models.PurchaseOrderLogs;

namespace Aldebaran.Application.Services.PurchaseOrderLogs
{
    /// <summary>Casos de uso de los logs de solicitudes de ajuste y de tolerancias de órdenes de compra.</summary>
    public interface IPurchaseOrderLogService
    {
        Task<PagedResult<PurchaseOrderAdjustmentRequestItem>> GetAdjustmentRequestsAsync(DateTime dateFrom, DateTime dateTo, string? searchKey, int skip, int take, CancellationToken ct = default);
        Task<PagedResult<ApprovalRangeLogItem>> GetApprovalRangeChangesAsync(DateTime dateFrom, DateTime dateTo, string? searchKey, int skip, int take, CancellationToken ct = default);
    }
}
