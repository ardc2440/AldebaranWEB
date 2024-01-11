using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IWarehouseTransferDetailRepository
    {
        Task<IEnumerable<WarehouseTransferDetail>> GetByWarehouseTransferIdAsync(int warehouseTransferId, CancellationToken ct = default);
    }
}
