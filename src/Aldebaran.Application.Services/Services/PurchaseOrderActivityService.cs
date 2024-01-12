using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services
{
    public class PurchaseOrderActivityService : IPurchaseOrderActivityService
    {
        private readonly IPurchaseOrderActivityRepository _repository;
        private readonly IMapper _mapper;
        public PurchaseOrderActivityService(IPurchaseOrderActivityRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IPurchaseOrderActivityRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task AddAsync(PurchaseOrderActivity purchaseOrderActivity, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.PurchaseOrderActivity>(purchaseOrderActivity) ?? throw new ArgumentNullException("Actividad de la orden de compra no puede ser nula.");
            await _repository.AddAsync(entity, ct);
        }

        public async Task DeleteAsync(int purchaseOrderActivityId, CancellationToken ct = default)
        {
            await _repository.DeleteAsync(purchaseOrderActivityId, ct);
        }

        public async Task<PurchaseOrderActivity?> FindAsync(int purchaseOrderActivityId, CancellationToken ct = default)
        {
            var data = await _repository.FindAsync(purchaseOrderActivityId, ct);
            return _mapper.Map<PurchaseOrderActivity?>(data);
        }

        public async Task<IEnumerable<PurchaseOrderActivity>> GetByPurchaseOrderIdAsync(int purchaseOrderId, CancellationToken ct = default)
        {
            var data = await _repository.GetByPurchaseOrderIdAsync(purchaseOrderId, ct);
            return _mapper.Map<IEnumerable<PurchaseOrderActivity>>(data);
        }

        public async Task UpdateAsync(int purchaseOrderActivityId, PurchaseOrderActivity purchaseOrderActivity, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.PurchaseOrderActivity>(purchaseOrderActivity) ?? throw new ArgumentNullException("Actividad de la orden de compra no puede ser nula.");
            await _repository.UpdateAsync(purchaseOrderActivityId, entity, ct);
        }
    }

}
