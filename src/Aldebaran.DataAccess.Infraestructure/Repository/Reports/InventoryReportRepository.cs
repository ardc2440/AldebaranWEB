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
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Set<InventoryReport>().FromSqlRaw($"EXEC SP_GET_INVENTORY_REPORT @ReferenceIds = '{referenceIdsFilter}'").ToListAsync(ct);
            }, ct);
        }
    }
}
