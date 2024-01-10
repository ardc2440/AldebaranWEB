using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

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

        public async Task<IEnumerable<PurchaseOrderActivity>> GetByPurchaseOrderIdAsync(int purchaseOrderId, CancellationToken ct = default)
        {
            var data = await _repository.GetByPurchaseOrderIdAsync(purchaseOrderId, ct);
            return _mapper.Map<IEnumerable<PurchaseOrderActivity>>(data);
        }
    }

}
