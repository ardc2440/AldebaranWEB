using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class ProviderReferenceReportRepository : RepositoryBase<AldebaranDbContext>, IProviderReferenceReportRepository
    {
        public ProviderReferenceReportRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<ProviderReferenceReport>> GetProviderReferenceReportDataAsync(string filter, CancellationToken ct)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Set<ProviderReferenceReport>().FromSqlRaw($"EXEC SP_GET_PROVIDER_REFERENCE_REPORT {filter}").ToListAsync(ct);
            }, ct);
        }
    }
}
