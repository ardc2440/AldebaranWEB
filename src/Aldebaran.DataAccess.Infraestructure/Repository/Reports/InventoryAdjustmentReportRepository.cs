using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class InventoryAdjustmentReportRepository : RepositoryBase<AldebaranDbContext>, IInventoryAdjustmentReportRepository
    {
        public InventoryAdjustmentReportRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<InventoryAdjustmentReport>> GetInventoryAdjustmentReportDataAsync(InventoryAdjustmentReportFilter filter, CancellationToken ct = default)
        {
            var command = ReportCommands.InventoryAdjustment(filter ?? new InventoryAdjustmentReportFilter());
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Set<InventoryAdjustmentReport>().FromSqlRaw(command.Sql, command.Parameters).AsNoTracking().ToListAsync(ct);
            }, ct);
        }
    }
}
