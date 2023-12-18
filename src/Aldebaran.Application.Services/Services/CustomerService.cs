using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

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
    }

}
