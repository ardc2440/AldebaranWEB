using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class AdjustmentTypeRepository : IAdjustmentTypeRepository
    {
        private readonly AldebaranDbContext _context;
        public AdjustmentTypeRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<AdjustmentType>> GetAsync(CancellationToken ct = default)
        {
            return await _context.AdjustmentTypes.AsNoTracking().ToListAsync(ct);
        }
    }

}
