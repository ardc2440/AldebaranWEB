using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services
{
    public class ForwarderAgentService : IForwarderAgentService
    {
        private readonly IForwarderAgentRepository _repository;
        private readonly IMapper _mapper;

        public ForwarderAgentService(IForwarderAgentRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IForwarderAgentRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
        public async Task<bool> ExistsByAgentName(string agentName, CancellationToken ct = default)
        {
            return await _repository.ExistsByAgentName(agentName, ct);
        }
        public async Task AddAsync(ForwarderAgent forwarderAgent, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.ForwarderAgent>(forwarderAgent) ?? throw new ArgumentNullException("Agente no puede ser nulo.");
            await _repository.AddAsync(entity, ct);
        }

        public async Task DeleteAsync(int forwarderAgentId, CancellationToken ct = default)
        {
            await _repository.DeleteAsync(forwarderAgentId, ct);
        }

        public async Task<ForwarderAgent?> FindAsync(int forwarderAgentId, CancellationToken ct = default)
        {
            var data = await _repository.FindAsync(forwarderAgentId, ct);
            return _mapper.Map<ForwarderAgent?>(data);
        }

        public async Task<IEnumerable<ForwarderAgent>> GetByForwarderIdAsync(int forwarderId, CancellationToken ct = default)
        {
            var data = await _repository.GetByForwarderIdAsync(forwarderId, ct);
            return _mapper.Map<List<ForwarderAgent>>(data);
        }

        public async Task UpdateAsync(int forwarderAgentId, ForwarderAgent forwarderAgent, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.ForwarderAgent>(forwarderAgent) ?? throw new ArgumentNullException("Agente no puede ser nulo.");
            await _repository.UpdateAsync(forwarderAgentId, entity, ct);
        }
    }
}
