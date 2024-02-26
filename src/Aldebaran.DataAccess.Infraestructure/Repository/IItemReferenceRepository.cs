using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IItemReferenceRepository
    {

        Task<ItemReference?> FindAsync(int itemReferenceId, CancellationToken ct = default);
        Task<IEnumerable<ItemReference>> GetByItemIdAsync(int itemId, CancellationToken ct = default);
        Task AddAsync(ItemReference itemReference, CancellationToken ct = default);
        Task UpdateAsync(int itemReferenceId, ItemReference itemReference, CancellationToken ct = default);
        Task DeleteAsync(int itemReferenceId, CancellationToken ct = default);
        Task<List<ItemReference>> GetAsync(CancellationToken ct = default);
        Task<List<ItemReference>> GetAsync(string searchKey, CancellationToken ct = default);
        Task<IEnumerable<ItemReference>> GetByStatusAsync(bool isActive, CancellationToken ct = default);
        Task<IEnumerable<ItemReference>> GetAllReferencesWithMinimumQuantity(CancellationToken ct = default);
        Task<IEnumerable<ItemReference>> GetAllReferencesOutOfStock(CancellationToken ct = default);

    }

}
