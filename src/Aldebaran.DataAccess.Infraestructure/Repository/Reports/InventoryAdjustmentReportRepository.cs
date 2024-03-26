using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class InventoryAdjustmentReportRepository : IInventoryAdjustmentReportRepository
    {
        private readonly AldebaranDbContext _context;
        public InventoryAdjustmentReportRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<InventoryAdjustmentReport>> GetInventoryAdjustmentReportDataAsync(CancellationToken ct = default)
        {
            return await _context.Set<InventoryAdjustmentReport>().ToListAsync(ct);
        }
    }
}
