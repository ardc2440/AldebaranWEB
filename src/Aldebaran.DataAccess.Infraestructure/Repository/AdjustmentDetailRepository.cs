using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class AdjustmentDetailRepository : RepositoryBase<AldebaranDbContext>, IAdjustmentDetailRepository
    {
        public AdjustmentDetailRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<AdjustmentDetail>> GetAsync(string searchKey, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.AdjustmentDetails.AsNoTracking()
                .Where(i => i.Warehouse.WarehouseName.Contains(searchKey) ||
                          i.ItemReference.Item.Line.LineName.Contains(searchKey) ||
                          i.ItemReference.Item.ItemName.Contains(searchKey) ||
                          i.ItemReference.Item.InternalReference.Contains(searchKey) ||
                          i.ItemReference.Item.Notes.Contains(searchKey) ||
                          i.ItemReference.Item.ProviderReference.Contains(searchKey) ||
                          i.ItemReference.Notes.Contains(searchKey) ||
                          i.ItemReference.ProviderReferenceCode.Contains(searchKey) ||
                          i.ItemReference.ProviderReferenceName.Contains(searchKey) ||
                          i.ItemReference.ReferenceCode.Contains(searchKey) ||
                          i.ItemReference.ReferenceName.Contains(searchKey))
                .Include(i => i.Adjustment)
                .Include(i => i.ItemReference.Item.Line)
                .Include(i => i.Warehouse)
                .ToListAsync(ct);
            }, ct);
        }

        public async Task<AdjustmentDetail?> FindAsync(int adjustmentDetailId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.AdjustmentDetails.AsNoTracking()
                .Include(i => i.Adjustment)
                .Include(i => i.ItemReference.Item.Line)
                .Include(i => i.Warehouse)
                .FirstOrDefaultAsync(w => w.AdjustmentDetailId == adjustmentDetailId, ct);
            }, ct);
        }

        public async Task<IEnumerable<AdjustmentDetail>> GetByAdjustmentIdAsync(int adjustmentId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.AdjustmentDetails.AsNoTracking()
                .Include(i => i.Adjustment)
                .Include(i => i.ItemReference.Item.Line)
                .Include(i => i.Warehouse)
                .Where(w => w.AdjustmentId == adjustmentId)
                .ToListAsync(ct);
            }, ct);
        }
    }

}
