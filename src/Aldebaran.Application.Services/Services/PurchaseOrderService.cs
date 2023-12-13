using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
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

        public async Task DeleteAsync(int purchaseOrderId, CancellationToken ct = default)
        {
            await _repository.DeleteAsync(purchaseOrderId, ct);
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
