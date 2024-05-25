using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class MeasureUnitRepository : RepositoryBase<AldebaranDbContext>, IMeasureUnitRepository
    {
        public MeasureUnitRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<MeasureUnit>> GetAsync(CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.MeasureUnits.AsNoTracking()
                           .ToListAsync(ct);
            }, ct);
        }
    }
}
