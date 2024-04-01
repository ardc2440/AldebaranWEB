using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class CustomerSaleReportRepository : ICustomerSaleReportRepository
    {
        private readonly AldebaranDbContext _context;
        public CustomerSaleReportRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<CustomerSaleReport>> GetCustomerSaleReportDataAsync(string filter = "", CancellationToken ct = default)
        {
            return await _context.Set<CustomerSaleReport>().FromSqlRaw($"EXEC SP_GET_CUSTOMER_SALES_REPORT {filter}").ToListAsync(ct);
        }
    }
}
