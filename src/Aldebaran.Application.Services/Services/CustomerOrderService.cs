using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services
{
    public class CustomerOrderService : ICustomerOrderService
    {
        private readonly ICustomerOrderRepository _repository;
        private readonly IMapper _mapper;
        public CustomerOrderService(ICustomerOrderRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(ICustomerOrderRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<CustomerOrder?> AddAsync(CustomerOrder customerOrder, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.CustomerOrder>(customerOrder) ?? throw new ArgumentNullException("Reserva no puede ser nula.");
            return _mapper.Map<CustomerOrder>(await _repository.AddAsync(entity, ct));
        }

        public async Task<IEnumerable<CustomerOrder>> GetAsync(CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(ct);
            return _mapper.Map<IEnumerable<CustomerOrder>>(data);
        }

        public async Task<IEnumerable<CustomerOrder>> GetAsync(string searchKey, CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(searchKey, ct);
            return _mapper.Map<IEnumerable<CustomerOrder>>(data);
        }

        public async Task<CustomerOrder?> FindAsync(int customerOrderId, CancellationToken ct = default)
        {
            var data = await _repository.FindAsync(customerOrderId, ct);
            return _mapper.Map<CustomerOrder?>(data);
        }

        public async Task CancelAsync(int customerOrderId, short canceledStatusDocumentId, Reason reason, CancellationToken ct = default)
        {
            var mapReason = _mapper.Map<DataAccess.Infraestructure.Models.Reason>(reason);
            await _repository.CancelAsync(customerOrderId, canceledStatusDocumentId, mapReason, ct);
        }

        public async Task UpdateAsync(int customerOrderId, CustomerOrder customerOrder, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.CustomerOrder>(customerOrder) ?? throw new ArgumentNullException("Reserva no puede ser nula.");
            await _repository.UpdateAsync(customerOrderId, entity, ct);
        }
    }

}
