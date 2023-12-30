using Aldebaran.Application.Services.Models;
namespace Aldebaran.Application.Services
{
    public interface IItemAreaService
    {
        Task<IEnumerable<ItemsArea>> GetByAreaIdAsync(short areaId, CancellationToken ct = default);
        Task AddAsync(ItemsArea item, CancellationToken ct = default);
        Task DeleteAsync(short areaId, int itemId, CancellationToken ct = default);
    }
}
