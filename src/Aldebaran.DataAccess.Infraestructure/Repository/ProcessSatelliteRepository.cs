using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ProcessSatelliteRepository : RepositoryBase<AldebaranDbContext>, IProcessSatelliteRepository
    {
        public ProcessSatelliteRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<ProcessSatellite?> FindAsync(int processSatelliteId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.ProcessSatellites.AsNoTracking()
                            .Include(i => i.IdentityType)
                            .Include(i => i.City.Department.Country)
                            .FirstOrDefaultAsync(i => i.ProcessSatelliteId == processSatelliteId, ct);
            }, ct);
        }

        public async Task<IEnumerable<ProcessSatellite>> GetAsync(CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.ProcessSatellites.AsNoTracking()
                            .Include(i => i.IdentityType)
                            .Include(i => i.City.Department.Country)
                            .ToListAsync(ct);
            }, ct);
        }
    }

}
