using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IWarehouseTransferDetailService
    {
        Task<IEnumerable<WarehouseTransferDetail>> GetByWarehouseTransferIdAsync(int warehouseTransferId, CancellationToken ct = default);
    }

}
