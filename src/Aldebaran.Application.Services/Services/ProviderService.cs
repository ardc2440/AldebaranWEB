using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services
{
    public class ProviderService : IProviderService
    {
        private readonly IProviderRepository _repository;
        private readonly IMapper _mapper;
        public ProviderService(IProviderRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IProviderRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
        public async Task<bool> ExistsByIdentificationNumber(string identificationNumber, CancellationToken ct = default)
        {
            return await _repository.ExistsByIdentificationNumber(identificationNumber, ct);
        }
        public async Task<bool> ExistsByCode(string code, CancellationToken ct = default)
        {
            return await _repository.ExistsByCode(code, ct);
        }
        public async Task<bool> ExistsByName(string name, CancellationToken ct = default)
        {
            return await _repository.ExistsByName(name, ct);
        }
        public async Task AddAsync(Provider provider, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.Provider>(provider) ?? throw new ArgumentNullException("Proveedor no puede ser nulo.");
            await _repository.AddAsync(entity, ct);
        }

        public async Task DeleteAsync(int providerId, CancellationToken ct = default)
        {
            await _repository.DeleteAsync(providerId, ct);
        }

        public async Task<Provider?> FindAsync(int providerId, CancellationToken ct = default)
        {
            var data = await _repository.FindAsync(providerId, ct);
            return _mapper.Map<Provider?>(data);
        }

        public async Task<(IEnumerable<Provider>, int)> GetAsync(int? skip = null, int? top = null, CancellationToken ct = default)
        {
            var (data, count) = await _repository.GetAsync(skip, top, ct);
            return (_mapper.Map<List<Provider>>(data), count);
        }

        public async Task<(IEnumerable<Provider>,int)> GetAsync(int skip, int top, string searchKey, CancellationToken ct = default)
        {
            var (data, count) = await _repository.GetAsync(skip, top, searchKey, ct);
            return (_mapper.Map<List<Provider>>(data), count);
        }

        public async Task UpdateAsync(int providerId, Provider provider, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.Provider>(provider) ?? throw new ArgumentNullException("Proveedor no puede ser nulo.");
            await _repository.UpdateAsync(providerId, entity, ct);

        }
    }

}
