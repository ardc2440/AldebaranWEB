using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class CustomerOrdersInProcessService : ICustomerOrdersInProcessService
    {
        private readonly ICustomerOrdersInProcessRepository _repository;
        private readonly IMapper _mapper;
        public CustomerOrdersInProcessService(ICustomerOrdersInProcessRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(ICustomerOrdersInProcessRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
    }

}
