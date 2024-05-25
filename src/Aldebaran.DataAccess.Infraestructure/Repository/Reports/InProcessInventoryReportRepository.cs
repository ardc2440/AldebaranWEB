using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class InProcessInventoryReportRepository : RepositoryBase<AldebaranDbContext>, IInProcessInventoryReportRepository
    {
        public InProcessInventoryReportRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<InProcessInventoryReport>> GetInProcessInventoryReportDataAsync(string referenceIdsFilter, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Set<InProcessInventoryReport>().FromSqlRaw($"EXEC SP_GET_IN_PROCESS_INVENTORY_REPORT @ReferenceIds = '{referenceIdsFilter}'").ToListAsync(ct);
            }, ct);
        }
    }
}
