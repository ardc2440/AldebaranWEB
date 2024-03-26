using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class InventoryReportRepository : IInventoryReportRepository
    {
        private readonly AldebaranDbContext _context;
        public InventoryReportRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<InventoryReport>> GetInventoryReportDataAsync(CancellationToken ct = default)
        {
            return await _context.Set<InventoryReport>().ToListAsync(ct);
        }
    }
}
