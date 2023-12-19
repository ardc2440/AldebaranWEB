using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

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

        public async Task<IEnumerable<PurchaseOrderDetail>> GetTransitDetailOrdersAsync(int referenceId, int statusOrder, CancellationToken ct = default)
        {
            var data = await _repository.GetTransitDetailOrdersAsync(referenceId, statusOrder, ct);
            return _mapper.Map<IEnumerable<PurchaseOrderDetail>>(data);
        }
    }
}
