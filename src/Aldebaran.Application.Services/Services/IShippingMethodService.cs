using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IShippingMethodService
    {
        Task<IEnumerable<ShippingMethod>> GetAsync(CancellationToken ct = default);
        Task<ShippingMethod?> FindAsync(short ShippingMethodId, CancellationToken ct = default);
    }

}
