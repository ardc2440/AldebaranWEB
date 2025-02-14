using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class AutomaticPurchaseOrderAssigmentReportRepository : RepositoryBase<AldebaranDbContext>, IAutomaticPurchaseOrderAssigmentReportRepository
    {
        public AutomaticPurchaseOrderAssigmentReportRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<AutomaticCustomerOrderAssigmentReport>> GetAutomaticCustomerOrderAssigmentReportDataAsync(string filter, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Set<AutomaticCustomerOrderAssigmentReport>().FromSqlRaw($"EXEC SP_AUTOMATIC_CUSTOMER_ORDER_ASSIGMENT_REPORT {filter}").ToListAsync(ct);
            }, ct);
        }
    }
}
