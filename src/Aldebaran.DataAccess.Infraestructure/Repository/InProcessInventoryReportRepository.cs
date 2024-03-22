using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class InProcessInventoryReportRepository : IInProcessInventoryReportRepository
    {
        private readonly AldebaranDbContext _context;
        public InProcessInventoryReportRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<InProcessInventoryReport>> GetInProcessInventoryReportDataAsync(CancellationToken ct = default)
        {
            return await _context.Set<InProcessInventoryReport>().ToListAsync(ct);            
        }
    }
}
