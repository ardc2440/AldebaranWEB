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
                
        public async Task<(IEnumerable<CustomerOrder>, int count)> GetWhitOutCancellationRequestAsync(int skip, int top, short editMode = -1, CancellationToken ct = default)
        {
            var (data, c) = await _repository.GetWhitOutCancellationRequestAsync(skip, top, editMode, ct);
            return (_mapper.Map<IEnumerable<CustomerOrder>>(data), c);
        }

        public async Task<(IEnumerable<CustomerOrder>, int count)> GetWhitOutCancellationRequestAsync(int skip, int top, string searchKey, short editMode = -1, CancellationToken ct = default)
        {
            var (data, c) = await _repository.GetWhitOutCancellationRequestAsync(skip, top, searchKey, editMode, ct);
            return (_mapper.Map<IEnumerable<CustomerOrder>>(data), c);
        }

        public async Task<(IEnumerable<CustomerOrder>, int count)> GetAsync(int skip, int top, short editMode = -1, CancellationToken ct = default)
        {
            var (data, c) = await _repository.GetAsync(skip, top, editMode, ct);
            return (_mapper.Map<IEnumerable<CustomerOrder>>(data), c);
        }

        public async Task<(IEnumerable<CustomerOrder>, int count)> GetAsync(int skip, int top, string searchKey, short editMode = -1, CancellationToken ct = default)
        {
            var (data, c) = await _repository.GetAsync(skip, top, searchKey, editMode, ct);
            return (_mapper.Map<IEnumerable<CustomerOrder>>(data), c);
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

        public async Task CloseAsync(int customerOrderId, short closedStatusDocumentId, Reason reason, CancellationToken ct = default)
        {
            var mapReason = _mapper.Map<DataAccess.Infraestructure.Models.Reason>(reason);
            await _repository.CloseAsync(customerOrderId, closedStatusDocumentId, mapReason, ct);
        }

        public async Task UpdateAsync(int customerOrderId, CustomerOrder customerOrder, Reason reason, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.CustomerOrder>(customerOrder) ?? throw new ArgumentNullException("Pedido no puede ser nulo.");
            var mapReason = _mapper.Map<DataAccess.Infraestructure.Models.Reason>(reason);
            await _repository.UpdateAsync(customerOrderId, entity, mapReason, ct);
        }

        /* Logs */
        public async Task<(IEnumerable<ModifiedCustomerOrder>, int count)> GetCustomerOrderModificationsLogAsync(int skip, int top, string searchKey, CancellationToken ct = default)
        {
            var (data, c) = await _repository.GetCustomerOrderModificationsLogAsync(skip, top, searchKey, ct);
            return (_mapper.Map<IEnumerable<ModifiedCustomerOrder>>(data), c);
        }
        public async Task<(IEnumerable<CanceledCustomerOrder>, int count)> GetCustomerOrderCancellationsLogAsync(int skip, int top, string searchKey, CancellationToken ct = default)
        {
            var (data, c) = await _repository.GetCustomerOrderCancellationsLogAsync(skip, top, searchKey, ct);
            return (_mapper.Map<IEnumerable<CanceledCustomerOrder>>(data), c);
        }
        public async Task<(IEnumerable<ClosedCustomerOrder>, int count)> GetCustomerOrderClosesLogAsync(int skip, int top, string searchKey, CancellationToken ct = default)
        {
            var (data, c) = await _repository.GetCustomerOrderClosesLogAsync(skip, top, searchKey, ct);
            return (_mapper.Map<IEnumerable<ClosedCustomerOrder>>(data), c);
        }
    }
}
