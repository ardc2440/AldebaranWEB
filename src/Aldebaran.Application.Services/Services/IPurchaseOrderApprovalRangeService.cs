using Aldebaran.Application.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.Application.Services.Services
{
    public interface IPurchaseOrderApprovalRangeService
    {
        Task<IEnumerable<PurchaseOrderApprovalRange>> GetAsync(CancellationToken ct = default);

        Task<PurchaseOrderApprovalRange?> FindAsync(int purchaseOrderApprovalRangeId, CancellationToken ct = default);

        Task AddAsync(PurchaseOrderApprovalRange entity, CancellationToken ct = default);

        Task UpdateAsync(int purchaseOrderApprovalRangeId, PurchaseOrderApprovalRange entity, CancellationToken ct = default);

        Task<PurchaseOrderApprovalResult> EvaluateAdjustmentAsync(int requestedQuantity, int receivedQuantity, CancellationToken ct = default);
    }
}
