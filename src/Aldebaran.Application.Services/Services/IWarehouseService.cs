using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IWarehouseService
    {
        Task<IEnumerable<Warehouse>> GetAsync(CancellationToken ct = default);
        Task<IEnumerable<Warehouse>> GetAsync(string searchKey, CancellationToken ct = default);
        Task<Warehouse?> FindAsync(short warehouseId, CancellationToken ct = default);
        Task<Warehouse?> FindByCodeAsync(short warehouseCode, CancellationToken ct = default);

    }

}
