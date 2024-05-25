using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class AdjustmentTypeRepository : RepositoryBase<AldebaranDbContext>, IAdjustmentTypeRepository
    {
        public AdjustmentTypeRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<AdjustmentType>> GetAsync(CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.AdjustmentTypes.AsNoTracking().ToListAsync(ct);
            }, ct);
        }
    }

}
