using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class AdjustmentReasonRepository : RepositoryBase<AldebaranDbContext>, IAdjustmentReasonRepository
    {
        public AdjustmentReasonRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        public async Task<IEnumerable<AdjustmentReason>> GetAsync(CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.AdjustmentReasons.AsNoTracking().ToListAsync(ct);
            }, ct);
        }
    }

}
