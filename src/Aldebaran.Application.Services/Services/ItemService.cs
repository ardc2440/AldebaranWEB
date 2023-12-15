using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _repository;
        private readonly IMapper _mapper;
        public ItemService(IItemRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IItemRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task AddAsync(Item item, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.Item>(item) ?? throw new ArgumentNullException("Artículo no puede ser nulo.");
            await _repository.AddAsync(entity, ct);
        }

        public async Task DeleteAsync(int itemId, CancellationToken ct = default)
        {
            await _repository.DeleteAsync(itemId, ct);
        }

        public async Task<Item?> FindAsync(int itemId, CancellationToken ct = default)
        {
            var data = await _repository.FindAsync(itemId, ct);
            return _mapper.Map<Item?>(data);
        }

        public async Task<IEnumerable<Item>> GetAsync(CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(ct);
            return _mapper.Map<List<Item>>(data);
        }

        public async Task<IEnumerable<Item>> GetAsync(string searchKey, CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(searchKey, ct);
            return _mapper.Map<List<Item>>(data);
        }

        public async Task UpdateAsync(int itemId, Item item, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.Item>(item) ?? throw new ArgumentNullException("Artículo no puede ser nulo.");
            await _repository.UpdateAsync(itemId, entity, ct);
        }
    }

}
