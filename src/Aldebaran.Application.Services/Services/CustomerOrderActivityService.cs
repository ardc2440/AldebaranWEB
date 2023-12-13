using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class CustomerOrderActivityService : ICustomerOrderActivityService
    {
        private readonly ICustomerOrderActivityRepository _repository;
        private readonly IMapper _mapper;
        public CustomerOrderActivityService(ICustomerOrderActivityRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(ICustomerOrderActivityRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
    }

}
