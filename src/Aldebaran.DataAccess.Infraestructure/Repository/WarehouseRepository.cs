using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class WarehouseRepository : RepositoryBase<AldebaranDbContext>, IWarehouseRepository
    {
        public WarehouseRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<List<Warehouse>> GetAsync(CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Warehouses.AsNoTracking()
               .ToListAsync(ct);
            }, ct);
        }

        public async Task<List<Warehouse>> GetAsync(string searchKey, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Warehouses.AsNoTracking()
                            .Where(i => i.WarehouseName.Contains(searchKey))
                            .ToListAsync(ct);
            }, ct);
        }

        public async Task<Warehouse?> FindAsync(short warehouseId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Warehouses.AsNoTracking()
                    .FirstOrDefaultAsync(i => i.WarehouseId == warehouseId);
            }, ct);
        }

        public async Task<Warehouse?> FindByCodeAsync(short warehouseCode, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Warehouses.AsNoTracking()
                    .FirstOrDefaultAsync(i => i.WarehouseCode == warehouseCode);
            }, ct);
        }
    }

}
