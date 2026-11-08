using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class InventoryReportRepository : RepositoryBase<AldebaranDbContext>, IInventoryReportRepository
    {
        public InventoryReportRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<InventoryReport>> GetInventoryReportDataAsync(string referenceIdsFilter, CancellationToken ct = default)
        {
            var command = ReportCommands.ByReferenceIds("dbo.SP_GET_INVENTORY_REPORT", referenceIdsFilter);
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Set<InventoryReport>().FromSqlRaw(command.Sql, command.Parameters).AsNoTracking().ToListAsync(ct);
            }, ct);
        }
    }
}
