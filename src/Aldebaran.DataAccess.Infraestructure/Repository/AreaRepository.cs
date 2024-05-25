using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class AreaRepository : RepositoryBase<AldebaranDbContext>, IAreaRepository
    {
        public AreaRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        public async Task<Area?> FindAsync(short areaId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Areas.AsNoTracking().FirstOrDefaultAsync(f => f.AreaId == areaId, ct);
            }, ct);
        }
        public async Task<IEnumerable<Area>> GetAsync(CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Areas.AsNoTracking().ToListAsync(ct);
            }, ct);
        }

        public async Task<IEnumerable<Area>> GetAsync(string searchKey, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Areas.AsNoTracking()
                   .Where(w => w.AreaCode.Contains(searchKey) || w.AreaName.Contains(searchKey) || w.Description.Contains(searchKey))
                   .ToListAsync(ct);
            }, ct);
        }
    }
}
