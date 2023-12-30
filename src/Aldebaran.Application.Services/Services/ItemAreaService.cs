using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Entities;
namespace Aldebaran.Application.Services
{
    public class ItemAreaService : IItemAreaService
    {
        private readonly IItemAreaRepository _repository;
        private readonly IMapper _mapper;

        public ItemAreaService(IItemAreaRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IItemAreaRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task AddAsync(ItemsArea item, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.ItemsArea>(item) ?? throw new ArgumentNullException("Artículo no puede ser nulo.");
            await _repository.AddAsync(entity, ct);
        }

        public async Task DeleteAsync(short areaId, int itemId, CancellationToken ct = default)
        {
            await _repository.DeleteAsync(areaId, itemId, ct);
        }
        public async Task<IEnumerable<ItemsArea>> GetByAreaIdAsync(short areaId, CancellationToken ct = default)
        {
            var data = await _repository.GetByAreaIdAsync(areaId, ct);
            return _mapper.Map<List<ItemsArea>>(data);
        }
    }
}
