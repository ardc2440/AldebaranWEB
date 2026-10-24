using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class CustomerSaleReportRepository : RepositoryBase<AldebaranDbContext>, ICustomerSaleReportRepository
    {
        public CustomerSaleReportRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<CustomerSaleReport>> GetCustomerSaleReportDataAsync(CustomerOrderReportFilter filter, CancellationToken ct = default)
        {
            var command = CustomerOrderReportCommand.Build("SP_GET_CUSTOMER_SALES_REPORT", filter);
            return await ExecuteQueryAsync(async dbContext =>
                await dbContext.Set<CustomerSaleReport>().FromSqlRaw(command.Sql, command.Parameters).ToListAsync(ct), ct);
        }
    }
}
