using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IItemService
    {
        Task<IEnumerable<Item>> GetAsync(CancellationToken ct = default);
        Task<Item?> FindAsync(int itemId, CancellationToken ct = default);
    }
}
