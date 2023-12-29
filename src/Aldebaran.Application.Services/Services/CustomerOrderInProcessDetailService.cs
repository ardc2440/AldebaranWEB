using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class CustomerOrderInProcessDetailService : ICustomerOrderInProcessDetailService
    {
        private readonly ICustomerOrderInProcessDetailRepository _repository;
        private readonly IMapper _mapper;
        public CustomerOrderInProcessDetailService(ICustomerOrderInProcessDetailRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(ICustomerOrderInProcessDetailRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<IEnumerable<CustomerOrderInProcessDetail>> GetAsync(int customerOrderInProcessId, CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(customerOrderInProcessId, ct);
            return _mapper.Map<IEnumerable<CustomerOrderInProcessDetail>>(data);
        }
    }
}
