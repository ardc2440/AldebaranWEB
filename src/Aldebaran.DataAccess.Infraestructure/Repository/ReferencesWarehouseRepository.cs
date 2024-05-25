using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ReferencesWarehouseRepository : RepositoryBase<AldebaranDbContext>, IReferencesWarehouseRepository
    {
        public ReferencesWarehouseRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<ReferencesWarehouse>> GetAllAsync(CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.ReferencesWarehouses.AsNoTracking()
                            .Include(x => x.ItemReference.Item.Line)
                            .Include(x => x.Warehouse)
                            .ToListAsync(ct);
            }, ct);
        }

        public async Task<ReferencesWarehouse?> GetByReferenceAndWarehouseIdAsync(int referenceId, short warehouseId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.ReferencesWarehouses.AsNoTracking()
                            .Include(x => x.ItemReference.Item.Line)
                            .Include(x => x.Warehouse)
                            .FirstOrDefaultAsync(x => x.ReferenceId == referenceId && x.WarehouseId == warehouseId, ct);
            }, ct);
        }

        public async Task<IEnumerable<ReferencesWarehouse>> GetByReferenceIdAsync(int referenceId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.ReferencesWarehouses.AsNoTracking()
                            .Include(x => x.ItemReference.Item.Line)
                            .Include(x => x.Warehouse)
                            .Where(x => x.ReferenceId == referenceId)
                            .ToListAsync(ct);
            }, ct);
        }
    }
}
