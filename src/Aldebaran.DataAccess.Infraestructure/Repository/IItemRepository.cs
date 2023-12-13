using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IItemRepository
    {
        Task<IEnumerable<Item>> GetAsync(CancellationToken ct = default);
        Task<Item?> FindAsync(int itemId, CancellationToken ct = default);
    }

}
