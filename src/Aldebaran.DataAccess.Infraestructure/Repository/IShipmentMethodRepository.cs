using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IShipmentMethodRepository
    {
        Task<IEnumerable<ShipmentMethod>> GetAsync(CancellationToken ct = default);
    }
}
