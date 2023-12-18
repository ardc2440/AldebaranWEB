using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IIdentityTypeRepository
    {
        Task<IEnumerable<IdentityType>> GetAsync(CancellationToken ct = default);
    }
}
