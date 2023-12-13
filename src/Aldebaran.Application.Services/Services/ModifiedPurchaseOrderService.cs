using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class ModifiedPurchaseOrderService : IModifiedPurchaseOrderService
    {
        private readonly IModifiedPurchaseOrderRepository _repository;
        private readonly IMapper _mapper;
        public ModifiedPurchaseOrderService(IModifiedPurchaseOrderRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IModifiedPurchaseOrderRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
    }

}
