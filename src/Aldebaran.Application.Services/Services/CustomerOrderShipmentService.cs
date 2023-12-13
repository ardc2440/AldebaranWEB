using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class CustomerOrderShipmentService : ICustomerOrderShipmentService
    {
        private readonly ICustomerOrderShipmentRepository _repository;
        private readonly IMapper _mapper;
        public CustomerOrderShipmentService(ICustomerOrderShipmentRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(ICustomerOrderShipmentRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
    }

}
