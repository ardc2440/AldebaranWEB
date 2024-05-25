using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ShippingMethodRepository : RepositoryBase<AldebaranDbContext>, IShippingMethodRepository
    {
        public ShippingMethodRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<ShippingMethod>> GetAsync(CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.ShippingMethods.AsNoTracking().ToListAsync(ct);

            }, ct);
        }
        public async Task<ShippingMethod?> FindAsync(short ShippingMethodId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.ShippingMethods.AsNoTracking()
               .FirstOrDefaultAsync(i => i.ShippingMethodId == ShippingMethodId, ct);
            }, ct);
        }
    }
}
