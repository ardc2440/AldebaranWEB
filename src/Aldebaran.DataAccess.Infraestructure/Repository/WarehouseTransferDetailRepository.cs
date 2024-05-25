using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class WarehouseTransferDetailRepository : RepositoryBase<AldebaranDbContext>, IWarehouseTransferDetailRepository
    {
        public WarehouseTransferDetailRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<WarehouseTransferDetail>> GetByWarehouseTransferIdAsync(int warehouseTransferId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.WarehouseTransferDetails.AsNoTracking()
                            .Include(i => i.ItemReference.Item.Line)
                            .Where(i => i.WarehouseTransferId == warehouseTransferId)
                            .ToListAsync(ct);
            }, ct);
        }
    }
}
