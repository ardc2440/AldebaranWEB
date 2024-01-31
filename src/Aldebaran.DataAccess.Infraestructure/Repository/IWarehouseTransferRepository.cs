using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IWarehouseTransferRepository
    {
        Task<WarehouseTransfer?> FindAsync(int warehouseTransferId, CancellationToken ct = default);
        Task CancelAsync(int warehouseTransferId, CancellationToken ct = default);
        Task<IEnumerable<WarehouseTransfer>> GetAsync(CancellationToken ct = default);
        Task<IEnumerable<WarehouseTransfer>> GetAsync(string search, CancellationToken ct = default);
        Task<WarehouseTransfer?> AddAsync(WarehouseTransfer warehouseTransfer, CancellationToken ct = default);        
    }
}
