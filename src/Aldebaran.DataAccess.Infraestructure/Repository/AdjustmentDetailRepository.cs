using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class AdjustmentDetailRepository : IAdjustmentDetailRepository
    {
        private readonly AldebaranDbContext _context;
        public AdjustmentDetailRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<AdjustmentDetail>> GetAsync(string filter, CancellationToken ct = default)
        {
            return await _context.AdjustmentDetails.AsNoTracking()
                .Where(filter)
                .Include(i => i.Adjustment)
                .Include(i => i.ItemReference.Item.Line)
                .Include(i => i.Warehouse)
                .ToListAsync(ct);
        }

        public async Task<AdjustmentDetail?> FindAsync(int adjustmentDetailId, CancellationToken ct = default)
        {
            return await _context.AdjustmentDetails.AsNoTracking()
                .Include(i => i.Adjustment)
                .Include(i => i.ItemReference.Item.Line)
                .Include(i => i.Warehouse)
                .FirstOrDefaultAsync(w => w.AdjustmentDetailId == adjustmentDetailId, ct);
        }
    }

}
