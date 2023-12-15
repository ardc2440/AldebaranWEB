using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IWarehouseService
    {
        Task<IEnumerable<Warehouse>> GetAsync(CancellationToken ct = default);
        Task<IEnumerable<Warehouse>> GetAsync(string filter, CancellationToken ct = default);
        Task<Warehouse?> FindAsync(int wareHouseId, CancellationToken ct = default);
    }

}
