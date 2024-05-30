using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class PackagingRepository : RepositoryBase<AldebaranDbContext>, IPackagingRepository
    {
        public PackagingRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<Packaging?> FindByItemId(int itemId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Packagings.AsNoTracking().FirstOrDefaultAsync(f => f.ItemId == itemId, ct);
            }, ct);
        }
    }
}
