using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class AdjustmentDetailRepository : IAdjustmentDetailRepository
    {
        private readonly AldebaranDbContext _context;
        public AdjustmentDetailRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<AdjustmentDetail>> GetAsync(string searchKey, CancellationToken ct = default)
        {
            return await _context.AdjustmentDetails.AsNoTracking()
                .Where(i => i.Warehouse.WarehouseName.Equals(searchKey) ||
                          i.ItemReference.Item.Line.LineName.Equals(searchKey) ||
                          i.ItemReference.Item.ItemName.Equals(searchKey) ||
                          i.ItemReference.Item.InternalReference.Equals(searchKey) ||
                          i.ItemReference.Item.Notes.Equals(searchKey) ||
                          i.ItemReference.Item.ProviderReference.Equals(searchKey) ||
                          i.ItemReference.Notes.Equals(searchKey) ||
                          i.ItemReference.ProviderReferenceCode.Equals(searchKey) ||
                          i.ItemReference.ProviderReferenceName.Equals(searchKey) ||
                          i.ItemReference.ReferenceCode.Equals(searchKey) ||
                          i.ItemReference.ReferenceName.Equals(searchKey))
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

        public async Task<IEnumerable<AdjustmentDetail>> GetByAdjustmentIdAsync(int adjustmentId, CancellationToken ct = default)
        {
            return await _context.AdjustmentDetails.AsNoTracking()
                .Include(i => i.Adjustment)
                .Include(i => i.ItemReference.Item.Line)
                .Include(i => i.Warehouse)
                .Where(w => w.AdjustmentId == adjustmentId)
                .ToListAsync(ct);
        }
    }

}
