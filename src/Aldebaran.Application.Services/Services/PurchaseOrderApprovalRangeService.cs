using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services.Services;
using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Infraestructure.Repository.Aldebaran.DataAccess.Infraestructure.Repository;

namespace Aldebaran.Application.Services
{
    public class PurchaseOrderApprovalRangeService : IPurchaseOrderApprovalRangeService
    {
        private readonly IPurchaseOrderApprovalRangeRepository _purchaseOrderApprovalRangeRepository;

        public PurchaseOrderApprovalRangeService(IPurchaseOrderApprovalRangeRepository purchaseOrderApprovalRangeRepository)
        {
            _purchaseOrderApprovalRangeRepository = purchaseOrderApprovalRangeRepository;
        }

        public async Task<IEnumerable<PurchaseOrderApprovalRange>> GetAsync(CancellationToken ct = default)
        {
            return await _purchaseOrderApprovalRangeRepository.GetAsync(ct);
        }

        public async Task<PurchaseOrderApprovalRange?> FindAsync(int purchaseOrderApprovalRangeId, CancellationToken ct = default)
        {
            return await _purchaseOrderApprovalRangeRepository.FindAsync(purchaseOrderApprovalRangeId, ct);
        }

        public async Task AddAsync(PurchaseOrderApprovalRange entity, CancellationToken ct = default)
        {
            await ValidateAsync(entity, ct: ct);

            await _purchaseOrderApprovalRangeRepository.AddAsync(entity, ct);
        }

        public async Task UpdateAsync(int purchaseOrderApprovalRangeId, PurchaseOrderApprovalRange entity, CancellationToken ct = default)
        {
            await ValidateAsync(entity, purchaseOrderApprovalRangeId, ct);

            await _purchaseOrderApprovalRangeRepository.UpdateAsync(purchaseOrderApprovalRangeId, entity, ct);
        }

        public async Task<PurchaseOrderApprovalResult> EvaluateAdjustmentAsync(int requestedQuantity, int receivedQuantity, CancellationToken ct = default)
        {
            var range = await _purchaseOrderApprovalRangeRepository.GetByRequestedQuantityAsync(requestedQuantity, ct) ?? throw new InvalidOperationException($"No existe una tolerancia configurada para la cantidad solicitada {requestedQuantity}.");

            var maximumAllowedDifference = (int)Math.Ceiling(requestedQuantity * range.AllowedDifferencePercent / 100M);

            var difference = Math.Abs(receivedQuantity - requestedQuantity);

            return new PurchaseOrderApprovalResult
            {
                PurchaseOrderApprovalRangeId = range.PurchaseOrderApprovalRangeId,
                RequestedQuantity = requestedQuantity,
                ReceivedQuantity = receivedQuantity,
                Difference = difference,
                MaximumAllowedDifference = maximumAllowedDifference,
                AllowedDifferencePercent = range.AllowedDifferencePercent,
                RequiresApproval = difference > maximumAllowedDifference
            };
        }

        private async Task ValidateAsync(PurchaseOrderApprovalRange entity, int? purchaseOrderApprovalRangeId = -1, CancellationToken ct = default)
        {
            if (entity.RequestedQuantityFrom <= 0)
                throw new InvalidOperationException("La cantidad desde debe ser mayor a cero.");

            if (entity.RequestedQuantityTo < entity.RequestedQuantityFrom)
                throw new InvalidOperationException("La cantidad hasta debe ser mayor o igual a la cantidad desde.");

            if (entity.AllowedDifferencePercent < 0)
                throw new InvalidOperationException("El porcentaje permitido no puede ser negativo.");

            if (entity.AllowedDifferencePercent > 100)
                throw new InvalidOperationException("El porcentaje permitido no puede ser mayor a 100.");
            
            var overlap = await _purchaseOrderApprovalRangeRepository.ExistsOverlapAsync(entity.RequestedQuantityFrom, entity.RequestedQuantityTo, purchaseOrderApprovalRangeId, ct);

            if (overlap)
                throw new InvalidOperationException("El rango se traslapa con otro rango activo.");
        }
    }
}

