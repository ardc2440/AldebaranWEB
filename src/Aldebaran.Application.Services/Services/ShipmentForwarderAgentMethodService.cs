using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services
{
    public class ShipmentForwarderAgentMethodService : IShipmentForwarderAgentMethodService
    {
        private readonly IShipmentForwarderAgentMethodRepository _repository;
        private readonly IMapper _mapper;

        public ShipmentForwarderAgentMethodService(IShipmentForwarderAgentMethodRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IShipmentForwarderAgentMethodRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
        public async Task AddAsync(ShipmentForwarderAgentMethod shipmentForwarderAgentMethod, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.ShipmentForwarderAgentMethod>(shipmentForwarderAgentMethod) ?? throw new ArgumentNullException("Método de envío no puede ser nulo.");
            await _repository.AddAsync(entity, ct);
        }

        public async Task DeleteAsync(int shipmentForwarderAgentMethodId, CancellationToken ct = default)
        {
            await _repository.DeleteAsync(shipmentForwarderAgentMethodId, ct);
        }

        public async Task<ShipmentForwarderAgentMethod?> FindAsync(int shipmentForwarderAgentMethodId, CancellationToken ct = default)
        {
            var data = await _repository.FindAsync(shipmentForwarderAgentMethodId, ct);
            return _mapper.Map<ShipmentForwarderAgentMethod?>(data);
        }

        public async Task<IEnumerable<ShipmentForwarderAgentMethod>> GetAsync(int forwarderAgentId, CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(forwarderAgentId, ct);
            return _mapper.Map<List<ShipmentForwarderAgentMethod>>(data);
        }

        public async Task UpdateAsync(int shipmentForwarderAgentMethodId, ShipmentForwarderAgentMethod shipmentForwarderAgentMethod, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.ShipmentForwarderAgentMethod>(shipmentForwarderAgentMethod) ?? throw new ArgumentNullException("Método de envío no puede ser nulo.");
            await _repository.UpdateAsync(shipmentForwarderAgentMethodId, entity, ct);
        }
    }
}
