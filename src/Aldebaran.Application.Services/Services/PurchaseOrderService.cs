using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IPurchaseOrderRepository _repository;
        private readonly IMapper _mapper;

        public PurchaseOrderService(IPurchaseOrderRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IPurchaseOrderRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<PurchaseOrder> AddAsync(PurchaseOrder item, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.PurchaseOrder>(item) ?? throw new ArgumentNullException("Orden no puede ser nula.");
            var result = await _repository.AddAsync(entity, ct);
            return _mapper.Map<PurchaseOrder>(result);
        }

        public async Task DeleteAsync(int purchaseOrderId, CancellationToken ct = default)
        {
            await _repository.DeleteAsync(purchaseOrderId, ct);
        }

        public async Task<PurchaseOrder?> FindAsync(int purchaseOrderId, CancellationToken ct = default)
        {
            var data = await _repository.FindAsync(purchaseOrderId, ct);
            return _mapper.Map<PurchaseOrder?>(data);
        }

        public async Task<IEnumerable<PurchaseOrder>> GetAsync(CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(ct);
            return _mapper.Map<List<PurchaseOrder>>(data);
        }

        public async Task<IEnumerable<PurchaseOrder>> GetAsync(string searchKey, CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(searchKey, ct);
            return _mapper.Map<List<PurchaseOrder>>(data);
        }
    }
}
