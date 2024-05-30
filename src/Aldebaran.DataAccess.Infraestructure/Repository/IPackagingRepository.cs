using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IPackagingRepository
    {
        Task<Packaging?> FindByItemId(int itemId, CancellationToken ct=default);
    }
}
