using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class CustomerReservationReportRepository : RepositoryBase<AldebaranDbContext>, ICustomerReservationReportRepository
    {
        public CustomerReservationReportRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<CustomerReservationReport>> GetCustomerReservationReportDataAsync(CustomerReservationReportFilter filter, CancellationToken ct = default)
        {
            var command = ReportCommands.CustomerReservation(filter ?? new CustomerReservationReportFilter());
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Set<CustomerReservationReport>().FromSqlRaw(command.Sql, command.Parameters).AsNoTracking().ToListAsync(ct);
            }, ct);
        }
    }
}
