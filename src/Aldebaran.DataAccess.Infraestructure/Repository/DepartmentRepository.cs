using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class DepartmentRepository : RepositoryBase<AldebaranDbContext>, IDepartmentRepository
    {
        public DepartmentRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<Department?> FindAsync(int departmentId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Departments.AsNoTracking()
                    .FirstOrDefaultAsync(f => f.DepartmentId == departmentId, ct);
            }, ct);
        }

        public async Task<IEnumerable<Department>> GetByCountryIdAsync(int countryId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Departments.AsNoTracking()
                 .Include(i => i.Country)
                 .Where(f => f.CountryId == countryId)
                 .ToListAsync(ct);
            }, ct);
        }
    }
}
