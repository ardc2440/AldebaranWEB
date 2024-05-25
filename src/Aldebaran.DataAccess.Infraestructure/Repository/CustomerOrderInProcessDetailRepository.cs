using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CustomerOrderInProcessDetailRepository : RepositoryBase<AldebaranDbContext>, ICustomerOrderInProcessDetailRepository
    {
        public CustomerOrderInProcessDetailRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<CustomerOrderInProcessDetail>> GetByCustomerOrderInProcessIdAsync(int customerOrderInProcessId, CancellationToken ct)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.CustomerOrderInProcessDetails.AsNoTracking()
               .Include(i => i.CustomerOrderDetail.ItemReference.Item.Line)
               .Include(i => i.Warehouse)
               .Where(i => i.CustomerOrderInProcessId == customerOrderInProcessId)
               .ToListAsync(ct);
            }, ct);
        }
    }

}
