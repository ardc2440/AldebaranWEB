using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IItemService
    {
        Task<(IEnumerable<Item>, int)> GetAsync(int skip, int top, CancellationToken ct = default);
        Task<(IEnumerable<Item>, int)> GetAsync(int skip, int top, string searchKey, CancellationToken ct = default);
        Task<IEnumerable<Item>> GetAsync(CancellationToken ct = default);
        Task<IEnumerable<Item>> GetAsync(string searchKey, CancellationToken ct = default);

        Task<Item?> FindAsync(int itemId, CancellationToken ct = default);
        Task<bool> ExistsByIternalReference(string internalReference, CancellationToken ct = default);
        Task<bool> ExistsByItemName(string itemName, CancellationToken ct = default);
        Task AddAsync(Item item, CancellationToken ct = default);
        Task UpdateAsync(int itemId, Item item, CancellationToken ct = default);
        Task DeleteAsync(int itemId, CancellationToken ct = default);
    }
}
