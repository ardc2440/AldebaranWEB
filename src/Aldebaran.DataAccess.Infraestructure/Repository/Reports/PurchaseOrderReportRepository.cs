using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class PurchaseOrderReportRepository : RepositoryBase<AldebaranDbContext>, IPurchaseOrderReportRepository
    {
        public PurchaseOrderReportRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<PurchaseOrderReport>> GetPurchaseOrderReportDataAsync(PurchaseOrderReportFilter filter, CancellationToken ct = default)
        {
            var command = ReportCommands.PurchaseOrder(filter ?? new PurchaseOrderReportFilter());
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Set<PurchaseOrderReport>().FromSqlRaw(command.Sql, command.Parameters).AsNoTracking().ToListAsync(ct);
            }, ct);
        }        
    }
}
