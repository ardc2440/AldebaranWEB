using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IItemReferenceService
    {
        Task<IEnumerable<ItemReference>> GetAsync(CancellationToken ct = default);
        Task<IEnumerable<ItemReference>> GetAsync(string filter, CancellationToken ct = default);
        Task<ItemReference?> FindAsync(int referenceId, CancellationToken ct = default);

    }

}
