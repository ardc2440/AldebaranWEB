using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class CustomerContactService : ICustomerContactService
    {
        private readonly ICustomerContactRepository _repository;
        private readonly IMapper _mapper;
        public CustomerContactService(ICustomerContactRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(ICustomerContactRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
    }

}
