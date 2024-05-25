using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CityRepository : RepositoryBase<AldebaranDbContext>, ICityRepository
    {
        public CityRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<City?> FindAsync(int cityId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Cities.AsNoTracking()
                    .Include(i => i.Department.Country)
                    .FirstOrDefaultAsync(f => f.CityId == cityId, ct);
            }, ct);
        }

        public async Task<IEnumerable<City>> GetByDepartmentIdAsync(int departmentId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Cities.AsNoTracking()
                .Include(i => i.Department.Country)
                .Where(w => w.DepartmentId == departmentId)
                .ToListAsync(ct);
            }, ct);
        }
    }
}
