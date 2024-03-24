using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
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
