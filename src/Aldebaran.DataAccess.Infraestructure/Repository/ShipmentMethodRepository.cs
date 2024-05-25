using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ShipmentMethodRepository : RepositoryBase<AldebaranDbContext>, IShipmentMethodRepository
    {
        public ShipmentMethodRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<ShipmentMethod>> GetAsync(CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.ShipmentMethods.ToListAsync(ct);
            }, ct);
        }
    }
}
