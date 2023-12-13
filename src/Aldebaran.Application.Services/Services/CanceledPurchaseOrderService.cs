using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class CanceledPurchaseOrderService : ICanceledPurchaseOrderService
    {
        private readonly ICanceledPurchaseOrderRepository _repository;
        private readonly IMapper _mapper;
        public CanceledPurchaseOrderService(ICanceledPurchaseOrderRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(ICanceledPurchaseOrderRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
    }

}
