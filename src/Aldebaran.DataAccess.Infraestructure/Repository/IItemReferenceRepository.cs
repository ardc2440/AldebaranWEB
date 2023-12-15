using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IItemReferenceRepository
    {

        Task<ItemReference?> FindAsync(int itemReferenceId, CancellationToken ct = default);
        Task<IEnumerable<ItemReference>> GetAsync(int itemId, CancellationToken ct = default);
        Task AddAsync(ItemReference itemReference, CancellationToken ct = default);
        Task UpdateAsync(int itemReferenceId, ItemReference itemReference, CancellationToken ct = default);
        Task DeleteAsync(int itemReferenceId, CancellationToken ct = default);
    }

}
