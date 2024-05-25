using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class InventoryAdjustmentReportRepository : RepositoryBase<AldebaranDbContext>, IInventoryAdjustmentReportRepository
    {
        public InventoryAdjustmentReportRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<InventoryAdjustmentReport>> GetInventoryAdjustmentReportDataAsync(string filter, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Set<InventoryAdjustmentReport>().FromSqlRaw($"EXEC SP_GET_INVENTORY_ADJUSTMENT_REPORT {filter}").ToListAsync(ct);
            }, ct);
        }
    }
}
