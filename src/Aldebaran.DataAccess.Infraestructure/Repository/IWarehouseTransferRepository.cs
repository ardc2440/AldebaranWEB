using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IWarehouseTransferRepository
    {
        Task<WarehouseTransfer?> FindAsync(int warehouseTransferId, CancellationToken ct = default);
        Task CancelAsync(int warehouseTransferId, CancellationToken ct = default);
        Task<(IEnumerable<WarehouseTransfer>, int)> GetAsync(int skip, int top, CancellationToken ct = default);
        Task<(IEnumerable<WarehouseTransfer>, int)> GetAsync(int skip, int top, string search, CancellationToken ct = default);
        Task<WarehouseTransfer?> AddAsync(WarehouseTransfer warehouseTransfer, CancellationToken ct = default);
        Task<(IEnumerable<WarehouseTransfer> warehouseTransfers, int count)> GetAsync(int skip, int take, string filter, string orderBy, CancellationToken ct = default);
    }
}
