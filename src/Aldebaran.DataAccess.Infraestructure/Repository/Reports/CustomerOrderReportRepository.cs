using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;


namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class CustomerOrderReportRepository : ICustomerOrderReportRepository
    {
        private readonly AldebaranDbContext _context;
        public CustomerOrderReportRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<IEnumerable<CustomerOrderReport>> GetCustomerOrderReportDataAsync(string filter, CancellationToken ct = default)
        {
            return await _context.Set<CustomerOrderReport>().FromSqlRaw($"EXEC SP_GET_CUSTOMER_ORDER_REPORT {filter}").ToListAsync(ct);
        }

        public async Task<IEnumerable<CustomerOrderExport>> GetCustomerOrderExportDataAsync(string filter, CancellationToken ct = default)
        {
            return await _context.Set<CustomerOrderExport>().FromSqlRaw($"EXEC SP_GET_CUSTOMER_ORDER_EXPORT {filter}").ToListAsync(ct);
        }
    }
}
