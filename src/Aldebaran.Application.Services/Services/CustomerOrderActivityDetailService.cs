using Aldebaran.Application.Services.Models;
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

        public async Task DeleteAsync(int customerOrderActivityDetailId, CancellationToken ct = default)
        {
            await _repository.DeleteAsync(customerOrderActivityDetailId, ct);
        }

        public async Task<IEnumerable<CustomerOrderActivityDetail>> GetAsync(int customerOrderActivityId, CancellationToken ct = default)
        {
            var data = _repository.GetAsync(customerOrderActivityId, ct);
            return _mapper.Map<IEnumerable<CustomerOrderActivityDetail>>(data);
        }
    }

}
