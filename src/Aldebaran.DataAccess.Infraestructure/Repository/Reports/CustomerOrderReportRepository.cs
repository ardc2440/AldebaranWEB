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
        public async Task<IEnumerable<CustomerOrderReport>> GetCustomerOrderReportDataAsync(CancellationToken ct = default)
        {
            return await _context.Set<CustomerOrderReport>().ToListAsync(ct);
        }
    }
}
