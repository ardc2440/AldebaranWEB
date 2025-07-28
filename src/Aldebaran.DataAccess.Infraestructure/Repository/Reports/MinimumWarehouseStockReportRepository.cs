using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class MinimumWarehouseStockReportRepository : RepositoryBase<AldebaranDbContext>, IMinimumWarehouseStockReportRepository
    {
        public MinimumWarehouseStockReportRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<MinimumWarehouseStockReport>> GetMinimumWarehouseStockReportDataAsync(string filter, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Set<MinimumWarehouseStockReport>().FromSqlRaw($"EXEC SP_GET_MINIMUM_WAREHOUSE_STOCK_REPORT {filter}").ToListAsync(ct);
            }, ct);
        }
    }
}