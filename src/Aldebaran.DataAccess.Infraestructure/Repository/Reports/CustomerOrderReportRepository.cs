using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class CustomerOrderReportRepository : RepositoryBase<AldebaranDbContext>, ICustomerOrderReportRepository
    {
        public CustomerOrderReportRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public Task<IEnumerable<CustomerOrderReport>> GetCustomerOrderReportDataAsync(CustomerOrderReportFilter filter, CancellationToken ct = default) =>
            QueryAsync<CustomerOrderReport>("SP_GET_CUSTOMER_ORDER_REPORT", filter, ct);

        public Task<IEnumerable<CustomerOrderExport>> GetCustomerOrderExportDataAsync(CustomerOrderReportFilter filter, CancellationToken ct = default) =>
            QueryAsync<CustomerOrderExport>("SP_GET_CUSTOMER_ORDER_EXPORT", filter, ct);

        public Task<IEnumerable<BackOrderReport>> GetBackOrderReportDataAsync(CustomerOrderReportFilter filter, CancellationToken ct = default) =>
            QueryAsync<BackOrderReport>("SP_GET_BACKORDER_REPORT", filter, ct);

        public Task<IEnumerable<BackOrderExport>> GetBackOrderExportDataAsync(CustomerOrderReportFilter filter, CancellationToken ct = default) =>
            QueryAsync<BackOrderExport>("SP_GET_BACKORDER_EXPORT", filter, ct);

        private async Task<IEnumerable<T>> QueryAsync<T>(string procedure, CustomerOrderReportFilter filter, CancellationToken ct) where T : class
        {
            var command = CustomerOrderReportCommand.Build(procedure, filter);
            return await ExecuteQueryAsync(async dbContext =>
                await dbContext.Set<T>().FromSqlRaw(command.Sql, command.Parameters).ToListAsync(ct), ct);
        }
    }
}
