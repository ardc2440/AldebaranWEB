using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;
        private readonly IMapper _mapper;
        public CustomerService(ICustomerRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(ICustomerRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<bool> ExistsByName(string name, CancellationToken ct = default)
        {
            return await _repository.ExistsByName(name, ct);
        }
        public async Task<bool> ExistsByIdentificationNumber(string identificationNumber, CancellationToken ct = default)
        {
            return await _repository.ExistsByIdentificationNumber(identificationNumber, ct);
        }

        public async Task AddAsync(Customer customer, CancellationToken ct = default)
        {
            var data = _mapper.Map<Entities.Customer>(customer);
            await _repository.AddAsync(data, ct);
        }

        public async Task DeleteAsync(int customerId, CancellationToken ct = default)
        {
            await _repository.DeleteAsync(customerId, ct);
        }

        public async Task<Customer?> FindAsync(int customerId, CancellationToken ct = default)
        {
            var data = await _repository.FindAsync(customerId, ct);
            return _mapper.Map<Customer?>(data);
        }

        public async Task<IEnumerable<Customer>> GetAsync(CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(ct);
            return _mapper.Map<List<Customer>>(data);
        }

        public async Task<IEnumerable<Customer>> GetAsync(string searchKey, CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(searchKey, ct);
            return _mapper.Map<List<Customer>>(data);
        }

        public async Task UpdateAsync(int customerId, Customer customer, CancellationToken ct = default)
        {
            var data = _mapper.Map<Entities.Customer>(customer);
            await _repository.UpdateAsync(customerId, data, ct);
        }
    }

}
