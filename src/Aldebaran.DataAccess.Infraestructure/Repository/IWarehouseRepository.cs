using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IWarehouseRepository
    {
        Task<List<Warehouse>> GetAsync(CancellationToken ct = default);
        Task<List<Warehouse>> GetAsync(string searchKey, CancellationToken ct = default);
        Task<Warehouse?> FindAsync(short warehouseId, CancellationToken ct = default);
        Task<Warehouse?> FindByCodeAsync(short warehouseId, CancellationToken ct = default);
    }

}
