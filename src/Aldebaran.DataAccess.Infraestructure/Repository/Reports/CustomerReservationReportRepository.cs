using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class CustomerReservationReportRepository : RepositoryBase<AldebaranDbContext>, ICustomerReservationReportRepository
    {
        public CustomerReservationReportRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<CustomerReservationReport>> GetCustomerReservationReportDataAsync(string filter, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Set<CustomerReservationReport>().FromSqlRaw($"EXEC SP_GET_CUSTOMER_RESERVATION_REPORT {filter}").ToListAsync(ct);
            }, ct);
        }
    }
}
