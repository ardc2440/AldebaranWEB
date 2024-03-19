using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IItemReferenceService
    {
        Task<ItemReference?> FindAsync(int itemReferenceId, CancellationToken ct = default);
        Task<IEnumerable<ItemReference>> GetByItemIdAsync(int itemId, CancellationToken ct = default);
        Task AddAsync(ItemReference itemReference, CancellationToken ct = default);
        Task UpdateAsync(int itemReferenceId, ItemReference itemReference, CancellationToken ct = default);
        Task DeleteAsync(int itemReferenceId, CancellationToken ct = default);
        Task<IEnumerable<ItemReference>> GetAsync(CancellationToken ct = default);
        Task<IEnumerable<ItemReference>> GetAsync(string searchKey, CancellationToken ct = default);
        Task<IEnumerable<ItemReference>> GetByStatusAsync(bool isActive, CancellationToken ct = default);
        Task<IEnumerable<ItemReference>> GetAllReferencesWithMinimumQuantity(CancellationToken ct = default);
        Task<IEnumerable<ItemReference>> GetAllReferencesOutOfStock(CancellationToken ct = default);
        Task<IEnumerable<ItemReference>> GetReportsReferences(short? lineId = null, int? itemId = null, int? referenceId = null, bool? isExternalInventory = null, CancellationToken ct = default);
    }
}
