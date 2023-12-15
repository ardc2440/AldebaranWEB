using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class ItemReferenceService : IItemReferenceService
    {
        private readonly IItemReferenceRepository _repository;
        private readonly IMapper _mapper;
        public ItemReferenceService(IItemReferenceRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IItemReferenceRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<IEnumerable<ItemReference>> GetAsync(CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(ct);
            return _mapper.Map<List<ItemReference>>(data);
        }

        public async Task<IEnumerable<ItemReference>> GetAsync(string filter, CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(filter, ct);
            return _mapper.Map<List<ItemReference>>(data);
        }

        public async Task<ItemReference?> FindAsync(int referenceId, CancellationToken ct = default)
        {
            var data = await _repository.FindAsync(referenceId, ct);
            return _mapper.Map<ItemReference?>(data);
        }
    }

}
