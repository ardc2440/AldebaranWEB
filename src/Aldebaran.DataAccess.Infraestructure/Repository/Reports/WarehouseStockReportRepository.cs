using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class WarehouseStockReportRepository : RepositoryBase<AldebaranDbContext>, IWarehouseStockReportRepository
    {
        public WarehouseStockReportRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<WarehouseStockReport>> GetWarehouseStockReportDataAsync(WarehouseStockReportFilter filter, CancellationToken ct = default)
        {
            var command = ReportCommands.WarehouseStock(filter ?? new WarehouseStockReportFilter());
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Set<WarehouseStockReport>().FromSqlRaw(command.Sql, command.Parameters).AsNoTracking().ToListAsync(ct);
            }, ct);
        }
    }
}
