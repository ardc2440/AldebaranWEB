using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services
{
    public class ForwarderService : IForwarderService
    {
        private readonly IForwarderRepository _repository;
        private readonly IMapper _mapper;

        public ForwarderService(IForwarderRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IForwarderRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task AddAsync(Forwarder forwarder, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.Forwarder>(forwarder) ?? throw new ArgumentNullException("Transportadora no puede ser nula.");
            await _repository.AddAsync(entity, ct);
        }

        public async Task DeleteAsync(int forwarderId, CancellationToken ct = default)
        {
            await _repository.DeleteAsync(forwarderId, ct);
        }

        public async Task<Forwarder?> FindAsync(int forwarderId, CancellationToken ct = default)
        {
            var data = await _repository.FindAsync(forwarderId, ct);
            return _mapper.Map<Forwarder?>(data);
        }

        public async Task<IEnumerable<Forwarder>> GetAsync(CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(ct);
            return _mapper.Map<List<Forwarder>>(data);
        }

        public async Task<IEnumerable<Forwarder>> GetAsync(string searchKey, CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(searchKey, ct);
            return _mapper.Map<List<Forwarder>>(data);
        }

        public async Task UpdateAsync(int forwarderId, Forwarder forwarder, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.Forwarder>(forwarder) ?? throw new ArgumentNullException("Transportadora no puede ser nula.");
            await _repository.UpdateAsync(forwarderId, entity, ct);
        }
    }
}
