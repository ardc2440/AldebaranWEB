using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class WarehouseStockReportRepository : RepositoryBase<AldebaranDbContext>, IWarehouseStockReportRepository
    {
        public WarehouseStockReportRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<WarehouseStockReport>> GetWarehouseStockReportDataAsync(string filter, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Set<WarehouseStockReport>().FromSqlRaw($"EXEC SP_GET_WAREHOUSE_STOCK_REPORT {filter}").ToListAsync(ct);
            }, ct);
        }
    }
}
