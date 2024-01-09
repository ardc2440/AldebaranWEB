using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Entities;

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

        public async Task DeleteAsync(int customerOrderActivityId, CancellationToken ct = default)
        {
            await _repository.DeleteAsync(customerOrderActivityId, ct);
        }

        public async Task<IEnumerable<CustomerOrderActivity>> GetByCustomerOrderIdAsync(int customerOrderId, CancellationToken ct = default)
        {
            var data = await _repository.GetByCustomerOrderIdAsync(customerOrderId, ct);
            return _mapper.Map<IEnumerable<CustomerOrderActivity>>(data);
        }

        public async Task AddAsync(CustomerOrderActivity customerOrderActivity, CancellationToken ct = default)
        {
            var data = _mapper.Map<Entities.CustomerOrderActivity>(customerOrderActivity);
            await _repository.AddAsync(data, ct);
        }

        public async Task<CustomerOrderActivity?> FindAsync(int customerOrderActivityId, CancellationToken ct = default)
        {
            var data = await _repository.FindAsync(customerOrderActivityId, ct);
            return _mapper.Map<CustomerOrderActivity?>(data);
        }

        public async Task UpdateAsync(int customerOrderActivityId, CustomerOrderActivity customerOrderActivity, CancellationToken ct = default)
        {
            var data = _mapper.Map<Entities.CustomerOrderActivity>(customerOrderActivity);
            await _repository.UpdateAsync(customerOrderActivityId, data, ct);
        }
    }

}
