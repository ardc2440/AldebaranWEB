using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IItemReferenceService
    {
        Task<ItemReference?> FindAsync(int itemReferenceId, CancellationToken ct = default);
        Task<IEnumerable<ItemReference>> GetAsync(int itemId, CancellationToken ct = default);
        Task AddAsync(ItemReference itemReference, CancellationToken ct = default);
        Task UpdateAsync(int itemReferenceId, ItemReference itemReference, CancellationToken ct = default);
        Task DeleteAsync(int itemReferenceId, CancellationToken ct = default);
        Task<IEnumerable<ItemReference>> GetAsync(CancellationToken ct = default);
        Task<IEnumerable<ItemReference>> GetAsync(string searchKey, CancellationToken ct = default);
        Task<IEnumerable<ItemReference>> GetByStatusAsync(bool isActive, CancellationToken ct = default);
    }

}
