using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services
{
    public class CustomerOrderShipmentService : ICustomerOrderShipmentService
    {
        private readonly ICustomerOrderShipmentRepository _repository;
        private readonly IMapper _mapper;
        public CustomerOrderShipmentService(ICustomerOrderShipmentRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(ICustomerOrderShipmentRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<CustomerOrderShipment> AddAsync(CustomerOrderShipment customerOrderShipment, CancellationToken ct = default)
        {
            var data = _mapper.Map<Entities.CustomerOrderShipment>(customerOrderShipment);
            var result = await _repository.AddAsync(data, ct);
            return _mapper.Map<CustomerOrderShipment>(result);
        }

        public async Task<IEnumerable<CustomerOrderShipment>> GetByCustomerOrderIdAsync(int customerOrderId, CancellationToken ct = default)
        {
            var data = await _repository.GetByCustomerOrderIdAsync(customerOrderId, ct);
            return _mapper.Map<IEnumerable<CustomerOrderShipment>>(data);
        }

        public async Task UpdateAsync(int customerOrderShipmentId, CustomerOrderShipment customerOrderShipment, CancellationToken ct = default)
        {
            var data = _mapper.Map<Entities.CustomerOrderShipment>(customerOrderShipment);
            await _repository.UpdateAsync(customerOrderShipmentId, data, ct);
        }

        public async Task<CustomerOrderShipment?> FindAsync(int customerOrderShipmentId, CancellationToken ct = default)
        {
            var data = await _repository.FindAsync(customerOrderShipmentId, ct);
            return _mapper.Map<CustomerOrderShipment?>(data);
        }

        public async Task CancelAsync(int customerOrderShipmentId, short canceledStatusDocumentId, Reason reason, CancellationToken ct = default)
        {
            var mapReason = _mapper.Map<DataAccess.Infraestructure.Models.Reason>(reason);
            await _repository.CancelAsync(customerOrderShipmentId, canceledStatusDocumentId, mapReason, ct);
        }
    }

}
