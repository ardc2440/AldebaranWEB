using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class AdjustmentRepository : IAdjustmentRepository
    {
        private readonly AldebaranDbContext _context;
        public AdjustmentRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddAsync(Adjustment adjustment, CancellationToken ct = default)
        {
            await _context.Adjustments.AddAsync(adjustment, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int adjustmentId, CancellationToken ct = default)
        {
            var entity = await _context.Adjustments.FirstOrDefaultAsync(x => x.AdjustmentId == adjustmentId, ct) ?? throw new KeyNotFoundException($"Ajuste con id {adjustmentId} no existe.");
            _context.Adjustments.Remove(entity);
            try
            {
                await _context.SaveChangesAsync(ct);
            }
            catch
            {
                _context.Entry(entity).State = EntityState.Unchanged;
                throw;
            }
        }

        public async Task<Adjustment?> FindAsync(int adjustmentId, CancellationToken ct = default)
        {
            return await _context.Adjustments.AsNoTracking()
                .Include(i => i.StatusDocumentType)
                .Include(i => i.AdjustmentReason)
                .Include(i => i.AdjustmentType)
                .Include(i => i.Employee)
                .Include(i => i.AdjustmentDetails)
                .FirstOrDefaultAsync(w => w.AdjustmentId == adjustmentId, ct);
        }

        public async Task<IEnumerable<Adjustment>> GetAsync(CancellationToken ct = default)
        {
            return await _context.Adjustments.AsNoTracking()
                .Include(i => i.StatusDocumentType)
                .Include(i => i.AdjustmentReason)
                .Include(i => i.AdjustmentType)
                .Include(i => i.Employee)
                .Include(i => i.AdjustmentDetails)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Adjustment>> GetAsync(string filter, CancellationToken ct = default)
        {
            return await _context.Adjustments.AsNoTracking()
                .Where(filter)
                .Include(i => i.StatusDocumentType)
                .Include(i => i.AdjustmentReason)
                .Include(i => i.AdjustmentType)
                .Include(i => i.Employee)
                .Include(i => i.AdjustmentDetails)
                .ToListAsync(ct);
        }

        public async Task UpdateAsync(int adjustmentId, Adjustment adjustment, CancellationToken ct = default)
        {
            var entity = await _context.Adjustments
                .Include(i => i.AdjustmentDetails)
                .FirstOrDefaultAsync(x => x.AdjustmentId == adjustmentId, ct) ?? throw new KeyNotFoundException($"Transportadora con id {adjustmentId} no existe.");

            entity.AdjustmentDate = adjustment.AdjustmentDate;
            entity.AdjustmentTypeId = adjustment.AdjustmentTypeId;
            entity.AdjustmentReasonId = adjustment.AdjustmentReasonId;
            entity.EmployeeId = adjustment.EmployeeId;
            entity.AdjustmentDetails = adjustment.AdjustmentDetails;
            entity.Notes = adjustment.Notes;
            entity.CreationDate = adjustment.CreationDate;

            await _context.SaveChangesAsync(ct);
        }
    }

}
