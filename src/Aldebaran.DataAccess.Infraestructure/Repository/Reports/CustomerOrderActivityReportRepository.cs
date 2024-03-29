
using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class CustomerOrderActivityReportRepository : ICustomerOrderActivityReportRepository
    {
        private readonly AldebaranDbContext _context;
        public CustomerOrderActivityReportRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<CustomerOrderActivityReport>> GetCustomerOrderActivityReportDataAsync(string filter = "", CancellationToken ct = default)
        {
            return await _context.Set<CustomerOrderActivityReport>().FromSqlRaw($"EXEC SP_GET_CUSTOMER_ORDER_ACTIVITY_REPORT {filter}").ToListAsync(ct);
        }
    }
}
