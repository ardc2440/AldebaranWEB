using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class CustomerSaleReportRepository : RepositoryBase<AldebaranDbContext>, ICustomerSaleReportRepository
    {
        public CustomerSaleReportRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<CustomerSaleReport>> GetCustomerSaleReportDataAsync(string filter = "", CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Set<CustomerSaleReport>().FromSqlRaw($"EXEC SP_GET_CUSTOMER_SALES_REPORT {filter}").ToListAsync(ct);
            }, ct);
        }
    }
}
