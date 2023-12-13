using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class CustomerOrderActivityDetailService : ICustomerOrderActivityDetailService
    {
        private readonly ICustomerOrderActivityDetailRepository _repository;
        private readonly IMapper _mapper;
        public CustomerOrderActivityDetailService(ICustomerOrderActivityDetailRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(ICustomerOrderActivityDetailRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
    }

}
