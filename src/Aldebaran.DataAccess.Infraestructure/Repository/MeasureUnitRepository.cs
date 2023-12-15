using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class MeasureUnitRepository : IMeasureUnitRepository
    {
        private readonly AldebaranDbContext _context;
        public MeasureUnitRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<IEnumerable<MeasureUnit>> GetAsync(CancellationToken ct = default)
        {
            return await _context.MeasureUnits.AsNoTracking()
               .ToListAsync(ct);
        }
    }
}
