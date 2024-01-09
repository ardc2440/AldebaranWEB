using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CityRepository : ICityRepository
    {
        private readonly AldebaranDbContext _context;
        public CityRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<City?> FindAsync(int cityId, CancellationToken ct = default)
        {
            return await _context.Cities.AsNoTracking()
                .Include(i => i.Department.Country)
                .FirstOrDefaultAsync(f => f.CityId == cityId, ct);
        }

        public async Task<IEnumerable<City>> GetByDepartmentIdAsync(int departmentId, CancellationToken ct = default)
        {
            return await _context.Cities.AsNoTracking()
                .Include(i => i.Department.Country)
                .Where(w => w.DepartmentId == departmentId)
                .ToListAsync(ct);
        }
    }
}
