using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IPurchaseOrderApprovalRangeRepository
    {
        Task<IEnumerable<PurchaseOrderApprovalRange>> GetAsync(CancellationToken ct = default);

        Task<PurchaseOrderApprovalRange?> FindAsync(int purchaseOrderApprovalRangeId, CancellationToken ct = default);

        Task AddAsync(PurchaseOrderApprovalRange purchaseOrderApprovalRange, CancellationToken ct = default);

        Task UpdateAsync(int purchaseOrderApprovalRangeId, PurchaseOrderApprovalRange purchaseOrderApprovalRange, string changeReason, int employeeId, CancellationToken ct = default);

        Task<bool> ExistsOverlapAsync(int requestedQuantityFrom, int requestedQuantityTo, int? purchaseOrderApprovalRangeId = null, CancellationToken ct = default);

        Task<PurchaseOrderApprovalRange?> GetByRequestedQuantityAsync(int requestedQuantity, CancellationToken ct = default);
    }
}
