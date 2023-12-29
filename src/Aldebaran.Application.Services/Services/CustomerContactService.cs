using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Entities;

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

        public async Task AddAsync(CustomerContact customer, CancellationToken ct = default)
        {
            var data = _mapper.Map<Entities.CustomerContact>(customer);
            await _repository.AddAsync(data, ct);
        }

        public async Task DeleteAsync(int customerContactId, CancellationToken ct = default)
        {
            await _repository.DeleteAsync(customerContactId, ct);
        }

        public async Task<CustomerContact?> FindAsync(int customerContactId, CancellationToken ct = default)
        {
            var data = await _repository.FindAsync(customerContactId, ct);
            return _mapper.Map<CustomerContact?>(data);
        }

        public async Task<IEnumerable<CustomerContact>> GetByCustomerIdAsync(int customerId, CancellationToken ct = default)
        {
            var data = await _repository.GetByCustomerIdAsync(customerId, ct);
            return _mapper.Map<IEnumerable<CustomerContact>>(data);
        }

        public async Task UpdateAsync(int customerContactId, CustomerContact customerContact, CancellationToken ct = default)
        {
            var data = _mapper.Map<Entities.CustomerContact>(customerContact);
            await _repository.UpdateAsync(customerContactId, data, ct);
        }
    }

}
