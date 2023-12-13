using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly AldebaranDbContext _context;
        public DepartmentRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Department?> FindAsync(int departmentId, CancellationToken ct = default)
        {
            return await _context.Departments.AsNoTracking()
                .FirstOrDefaultAsync(f => f.DepartmentId == departmentId, ct);
        }

        public async Task<IEnumerable<Department>> GetAsync(int countryId, CancellationToken ct = default)
        {
            return await _context.Departments.AsNoTracking()
                .Include(i => i.Country)
                .Where(f => f.CountryId == countryId)
                .ToListAsync(ct);
        }
    }
}
