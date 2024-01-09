using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ProcessSatelliteRepository : IProcessSatelliteRepository
    {
        private readonly AldebaranDbContext _context;
        public ProcessSatelliteRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ProcessSatellite?> FindAsync(int processSatelliteId, CancellationToken ct = default)
        {
            return await _context.ProcessSatellites.AsNoTracking()
                .Include(i => i.IdentityType)
                .Include(i => i.City.Department.Country)
                .FirstOrDefaultAsync(i => i.ProcessSatelliteId == processSatelliteId, ct);
        }

        public async Task<IEnumerable<ProcessSatellite>> GetAsync(CancellationToken ct = default)
        {
            return await _context.ProcessSatellites.AsNoTracking()
                .Include(i => i.IdentityType)
                .Include(i => i.City.Department.Country)
                .ToListAsync(ct); ;
        }
    }

}
