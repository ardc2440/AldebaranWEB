using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    /// <summary>Consulta de solo lectura de los logs de solicitudes de ajuste y de tolerancias de órdenes de compra.</summary>
    public interface IPurchaseOrderLogRepository
    {
        Task<IEnumerable<PurchaseOrderAdjustmentRequestEntry>> GetAdjustmentRequestsAsync(DateTime dateFrom, DateTime dateTo, string? searchKey, int skip, int take, CancellationToken ct = default);
        Task<IEnumerable<ApprovalRangeLogEntry>> GetApprovalRangeChangesAsync(DateTime dateFrom, DateTime dateTo, string? searchKey, int skip, int take, CancellationToken ct = default);
    }
}
