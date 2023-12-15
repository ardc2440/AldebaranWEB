using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class AdjustmentReasonRepository : IAdjustmentReasonRepository
    {
        private readonly AldebaranDbContext _context;
        public AdjustmentReasonRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<IEnumerable<AdjustmentReason>> GetAsync(CancellationToken ct = default)
        {
            return await _context.AdjustmentReasons.AsNoTracking().ToListAsync(ct);
        }
    }

}
