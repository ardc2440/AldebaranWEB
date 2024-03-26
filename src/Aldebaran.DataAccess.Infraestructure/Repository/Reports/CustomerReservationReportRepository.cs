using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class CustomerReservationReportRepository : ICustomerReservationReportRepository
    {
        private readonly AldebaranDbContext _context;
        public CustomerReservationReportRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<IEnumerable<CustomerReservationReport>> GetCustomerReservationReportDataAsync(CancellationToken ct = default)
        {
            return await _context.Set<CustomerReservationReport>().ToListAsync(ct);
        }
    }
}
