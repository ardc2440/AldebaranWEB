using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class FreezoneVsAvailableReportRepository : RepositoryBase<AldebaranDbContext>, IFreezoneVsAvailableReportRepository
    {
        public FreezoneVsAvailableReportRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<FreezoneVsAvailableReport>> GetFreezoneVsAvailableReportDataAsync(string filter = "", CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Set<FreezoneVsAvailableReport>().FromSqlRaw($"EXEC SP_GET_FREEZONE_VS_AVAILABLE_REPORT @ReferenceIds = '{filter}'").ToListAsync(ct);
            }, ct);
        }
    }
}
