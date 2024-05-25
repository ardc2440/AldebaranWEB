using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class ReferenceMovementReportRepository : RepositoryBase<AldebaranDbContext>, IReferenceMovementReportRepository
    {
        public ReferenceMovementReportRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<ReferenceMovementReport>> GetReferenceMovementReportDataAsync(string filter, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Set<ReferenceMovementReport>().FromSqlRaw($"EXEC SP_GET_REFERENCE_MOVEMENT_REPORT {filter}").ToListAsync(ct);
            }, ct);
        }
    }
}
