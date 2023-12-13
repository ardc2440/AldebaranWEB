using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class CanceledCustomerOrderService : ICanceledCustomerOrderService
    {
        private readonly ICanceledCustomerOrderRepository _repository;
        private readonly IMapper _mapper;
        public CanceledCustomerOrderService(ICanceledCustomerOrderRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(ICanceledCustomerOrderRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
    }

}
