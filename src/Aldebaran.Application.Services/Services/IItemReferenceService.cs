using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IItemReferenceService
    {
        Task<ItemReference?> FindAsync(int itemReferenceId, CancellationToken ct = default);
        Task<bool> ExistsByReferenceCode(string referenceCode, CancellationToken ct = default);
        Task<bool> ExistsByReferenceName(string referenceName, CancellationToken ct = default);
        Task<IEnumerable<ItemReference>> GetByItemIdAsync(int itemId, CancellationToken ct = default);
        Task AddAsync(ItemReference itemReference, CancellationToken ct = default);
        Task UpdateAsync(int itemReferenceId, ItemReference itemReference, CancellationToken ct = default);
        Task DeleteAsync(int itemReferenceId, CancellationToken ct = default);
        Task<IEnumerable<ItemReference>> GetAsync(CancellationToken ct = default);
        Task<IEnumerable<ItemReference>> GetAsync(string searchKey, CancellationToken ct = default);
        Task<IEnumerable<ItemReference>> GetByStatusAsync(bool isActive, CancellationToken ct = default);
        Task<IEnumerable<ItemReference>> GetAllReferencesWithMinimumQuantityAsync(CancellationToken ct = default);
        Task<IEnumerable<ItemReference>> GetAllReferencesOutOfStockAsync(CancellationToken ct = default);
        Task<IEnumerable<ItemReference>> GetReportsReferencesAsync(bool? isReferenceActive = null, bool? isItemActive = null, bool? isExternalInventory = null, CancellationToken ct = default);
    }
}
