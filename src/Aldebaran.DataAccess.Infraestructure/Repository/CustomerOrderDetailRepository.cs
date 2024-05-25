using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CustomerOrderDetailRepository : RepositoryBase<AldebaranDbContext>, ICustomerOrderDetailRepository
    {
        public CustomerOrderDetailRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<CustomerOrderDetail>> GetByCustomerOrderIdAsync(int customerOrderId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.CustomerOrderDetails.AsNoTracking()
                                .Include(i => i.ItemReference.Item.Line)
                                .Where(i => i.CustomerOrderId == customerOrderId)
                                .ToListAsync(ct);
            }, ct);
        }
    }

}
