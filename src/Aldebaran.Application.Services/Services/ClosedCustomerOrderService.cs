using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class ClosedCustomerOrderService : IClosedCustomerOrderService
    {
        private readonly IClosedCustomerOrderRepository _repository;
        private readonly IMapper _mapper;
        public ClosedCustomerOrderService(IClosedCustomerOrderRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IClosedCustomerOrderRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
    }

}
