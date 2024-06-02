using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class PurchaseOrderReportRepository : RepositoryBase<AldebaranDbContext>, IPurchaseOrderReportRepository
    {
        public PurchaseOrderReportRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<PurchaseOrderReport>> GetPurchaseOrderReportDataAsync(string filter, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Set<PurchaseOrderReport>().FromSqlRaw($"EXEC SP_GET_PURCHASE_ORDER_REPORT {filter}").ToListAsync(ct);
            }, ct);
        }
    }
}
