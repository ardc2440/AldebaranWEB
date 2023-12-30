using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services
{
    public class ProviderReferenceService : IProviderReferenceService
    {
        private readonly IProviderReferenceRepository _repository;
        private readonly IMapper _mapper;
        public ProviderReferenceService(IProviderReferenceRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IProviderReferenceRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task AddAsync(ProviderReference providerReference, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.ProviderReference>(providerReference) ?? throw new ArgumentNullException("Referencia del proveedor no puede ser nula.");
            await _repository.AddAsync(entity, ct);
        }

        public async Task DeleteAsync(int providerId, int referenceId, CancellationToken ct = default)
        {
            await _repository.DeleteAsync(providerId, referenceId, ct);
        }

        public async Task<ProviderReference?> FindAsync(int providerId, int referenceId, CancellationToken ct = default)
        {
            var data = await _repository.FindAsync(providerId, referenceId, ct);
            return _mapper.Map<ProviderReference?>(data);
        }

        public async Task<IEnumerable<ProviderReference>> GetByProviderIdAsync(int providerId, CancellationToken ct = default)
        {
            var data = await _repository.GetByProviderIdAsync(providerId, ct);
            return _mapper.Map<List<ProviderReference>>(data);
        }
    }

}
