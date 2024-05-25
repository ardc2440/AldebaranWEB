using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class CustomerOrderReportRepository : RepositoryBase<AldebaranDbContext>, ICustomerOrderReportRepository
    {
        public CustomerOrderReportRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<CustomerOrderReport>> GetCustomerOrderReportDataAsync(string filter, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Set<CustomerOrderReport>().FromSqlRaw($"EXEC SP_GET_CUSTOMER_ORDER_REPORT {filter}").ToListAsync(ct);
            }, ct);
        }

        public async Task<IEnumerable<CustomerOrderExport>> GetCustomerOrderExportDataAsync(string filter, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Set<CustomerOrderExport>().FromSqlRaw($"EXEC SP_GET_CUSTOMER_ORDER_EXPORT {filter}").ToListAsync(ct);
            }, ct);
        }
    }
}
