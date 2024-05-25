using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class IdentityTypeRepository : RepositoryBase<AldebaranDbContext>, IIdentityTypeRepository
    {
        public IdentityTypeRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<IdentityType>> GetAsync(CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.IdentityTypes.AsNoTracking()
            .ToListAsync(ct);
            }, ct);
        }
    }
}
