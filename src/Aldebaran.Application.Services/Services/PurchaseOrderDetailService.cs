using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services
{
    public class PurchaseOrderDetailService : IPurchaseOrderDetailService
    {
        private readonly IPurchaseOrderDetailRepository _repository;
        private readonly IMapper _mapper;
        public PurchaseOrderDetailService(IPurchaseOrderDetailRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IPurchaseOrderDetailRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task AddAsync(PurchaseOrderDetail purchaseOrder, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.PurchaseOrderDetail>(purchaseOrder) ?? throw new ArgumentNullException("Referencia de la orden de compra no puede ser nula.");
            await _repository.AddAsync(entity, ct);
        }

        public async Task DeleteAsync(int purchaseOrderDetailId, CancellationToken ct = default)
        {
            await _repository.DeleteAsync(purchaseOrderDetailId, ct);
        }

        public async Task<PurchaseOrderDetail?> FindAsync(int purchaseOrderDetailId, CancellationToken ct = default)
        {
            var data = await _repository.FindAsync(purchaseOrderDetailId, ct);
            return _mapper.Map<PurchaseOrderDetail?>(data);
        }

        public async Task<IEnumerable<PurchaseOrderDetail>> GetByPurchaseOrderIdAsync(int purchaseOrderId, CancellationToken ct = default)
        {
            var data = await _repository.GetByPurchaseOrderIdAsync(purchaseOrderId, ct);
            return _mapper.Map<IEnumerable<PurchaseOrderDetail>>(data);
        }

        public async Task<IEnumerable<PurchaseOrderDetail>> GetTransitDetailOrdersAsync(int statusOrder, int? referenceId = null, CancellationToken ct = default)
        {
            var data = await _repository.GetByReferenceIdAndStatusOrderAsync(statusOrder, referenceId, ct);
            return _mapper.Map<IEnumerable<PurchaseOrderDetail>>(data);
        }

        public async Task UpdateAsync(int purchaseOrderDetailId, PurchaseOrderDetail purchaseOrder, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.PurchaseOrderDetail>(purchaseOrder) ?? throw new ArgumentNullException("Referencia de la orden de compra no puede ser nula.");
            await _repository.UpdateAsync(purchaseOrderDetailId, entity, ct);
        }
    }
}
