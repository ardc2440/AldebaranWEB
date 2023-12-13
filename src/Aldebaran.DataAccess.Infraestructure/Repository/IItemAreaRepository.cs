using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IItemAreaRepository
    {
        Task<IEnumerable<ItemsArea>> GetAsync(short areaId, CancellationToken ct = default);
        Task AddAsync(ItemsArea item, CancellationToken ct = default);
        Task DeleteAsync(short areaId, int itemId, CancellationToken ct = default);
    }
}
