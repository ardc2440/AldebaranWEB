using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services
{
    public class CustomerOrdersInProcessService : ICustomerOrdersInProcessService
    {
        private readonly ICustomerOrdersInProcessRepository _repository;
        private readonly IMapper _mapper;
        public CustomerOrdersInProcessService(ICustomerOrdersInProcessRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(ICustomerOrdersInProcessRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<CustomerOrdersInProcess> AddAsync(CustomerOrdersInProcess customerOrderInProcess, CancellationToken ct = default)
        {
            var data = _mapper.Map<Entities.CustomerOrdersInProcess>(customerOrderInProcess);
            var result = await _repository.AddAsync(data, ct);
            return _mapper.Map<CustomerOrdersInProcess>(result);
        }

        public async Task<IEnumerable<CustomerOrdersInProcess>> GetByCustomerOrderIdAsync(int customerOrderId, CancellationToken ct = default)
        {
            var data = await _repository.GetByCustomerOrderIdAsync(customerOrderId, ct);
            return _mapper.Map<IEnumerable<CustomerOrdersInProcess>>(data);
        }

        public async Task UpdateAsync(int customerOrderInProcessId, CustomerOrdersInProcess customerOrderInProcess, Reason reason, CancellationToken ct = default)
        {
            var data = _mapper.Map<Entities.CustomerOrdersInProcess>(customerOrderInProcess);
            var mapReason = _mapper.Map<DataAccess.Infraestructure.Models.Reason>(reason);
            await _repository.UpdateAsync(customerOrderInProcessId, data, mapReason, ct);
        }

        public async Task<CustomerOrdersInProcess?> FindAsync(int customerOrderInProcessId, CancellationToken ct = default)
        {
            var data = await _repository.FindAsync(customerOrderInProcessId, ct);
            return _mapper.Map<CustomerOrdersInProcess?>(data);
        }

        public async Task CancelAsync(int customerOrderInProcessId, short canceledStatusDocumentId, Reason reason, CancellationToken ct = default)
        {
            var mapReason = _mapper.Map<DataAccess.Infraestructure.Models.Reason>(reason);
            await _repository.CancelAsync(customerOrderInProcessId, canceledStatusDocumentId, mapReason, ct);
        }
    }

}
