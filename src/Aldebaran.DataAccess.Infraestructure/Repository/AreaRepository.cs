using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class AreaRepository : IAreaRepository
    {
        private readonly AldebaranDbContext _context;
        public AreaRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<Area?> FindAsync(short areaId, CancellationToken ct = default)
        {
            return await _context.Areas.AsNoTracking().FirstOrDefaultAsync(f => f.AreaId == areaId, ct);
        }
        public async Task<IEnumerable<Area>> GetAsync(CancellationToken ct = default)
        {
            return await _context.Areas.AsNoTracking().ToListAsync(ct);
        }

        public async Task<IEnumerable<Area>> GetAsync(string searchKey, CancellationToken ct = default)
        {
            return await _context.Areas.AsNoTracking()
               .Where(w => w.AreaCode.Contains(searchKey) || w.AreaName.Contains(searchKey) || w.Description.Contains(searchKey))
               .ToListAsync(ct);
        }
    }
}
