using Aldebaran.Application.Services.Models;
using Entities = Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services.Services
{
    public class InventoryAutomationConnectionService : IInventoryAutomationConnectionService
    {
        private readonly IInventoryAutomationConnectionRepository _repository;
        private readonly IMapper _mapper;

        public InventoryAutomationConnectionService(IInventoryAutomationConnectionRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IInventoryAutomationConnectionRepository)); 
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper)); 
        }
        
        public async Task<InventoryAutomationConnection> CreateAsync(InventoryAutomationConnection model, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.InventoryAutomationConnection>(model);
            var result = await _repository.AddAsync(entity, ct);
            return _mapper.Map<InventoryAutomationConnection>(result);
        }

        public async Task<InventoryAutomationConnection> UpdateAsync(InventoryAutomationConnection model, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.InventoryAutomationConnection>(model);
            var result = await _repository.UpdateAsync(entity, ct);
            return _mapper.Map<InventoryAutomationConnection>(result);
        }

        public async Task<InventoryAutomationConnection> ChangeActivationAsync(int id, bool active, CancellationToken ct = default)
        {
            await _repository.ChangeActivationAsync(id, active, ct);
            return await GetByIdAsync(id, ct);
        }

        public async Task<IEnumerable<InventoryAutomationConnection>> GetAllAsync(CancellationToken ct = default)
        {
            var result = await _repository.GetAllAsync(ct);
            return _mapper.Map<IEnumerable<InventoryAutomationConnection>>(result);
        }

        public async Task<InventoryAutomationConnection> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var result = await _repository.GetByIdAsync(id, ct);
            return _mapper.Map<InventoryAutomationConnection>(result);
        }
    }
}