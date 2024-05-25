using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class CustomerOrderActivityReportRepository : RepositoryBase<AldebaranDbContext>, ICustomerOrderActivityReportRepository
    {
        public CustomerOrderActivityReportRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<CustomerOrderActivityReport>> GetCustomerOrderActivityReportDataAsync(string filter = "", CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Set<CustomerOrderActivityReport>().FromSqlRaw($"EXEC SP_GET_CUSTOMER_ORDER_ACTIVITY_REPORT {filter}").ToListAsync(ct);
            }, ct);
        }
    }
}
