using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IShipmentMethodService
    {
        Task<IEnumerable<ShipmentMethod>> GetAsync(CancellationToken ct = default);
    }
}
