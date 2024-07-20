using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IWarehouseTransferService
    {
        Task<WarehouseTransfer?> FindAsync(int warehouseTransferId, CancellationToken ct = default);
        Task<(IEnumerable<WarehouseTransfer>, int)> GetAsync(int skip, int top, CancellationToken ct = default);
        Task<(IEnumerable<WarehouseTransfer>, int)> GetAsync(int skip, int top, string search, CancellationToken ct = default);
        Task<WarehouseTransfer?> AddAsync(WarehouseTransfer warehouseTransfer, CancellationToken ct = default);
        Task CancelAsync(int warehouseTransferId, CancellationToken ct = default);
    }
}
