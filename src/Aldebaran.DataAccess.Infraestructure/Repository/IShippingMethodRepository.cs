using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IShippingMethodRepository
    {
        Task<IEnumerable<ShippingMethod>> GetAsync(CancellationToken ct = default);
        Task<ShippingMethod?> FindAsync(short ShippingMethodId, CancellationToken ct = default);
    }

}
