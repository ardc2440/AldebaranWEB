using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IItemReferenceRepository
    {
        Task<List<ItemReference>> GetAsync(CancellationToken ct = default);
        Task<List<ItemReference>> GetAsync(string filter, CancellationToken ct = default);
        Task<ItemReference?> FindAsync(int referenceId, CancellationToken ct = default);
    }

}
