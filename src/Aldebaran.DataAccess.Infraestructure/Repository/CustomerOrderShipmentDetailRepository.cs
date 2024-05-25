using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CustomerOrderShipmentDetailRepository : RepositoryBase<AldebaranDbContext>, ICustomerOrderShipmentDetailRepository
    {
        public CustomerOrderShipmentDetailRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<CustomerOrderShipmentDetail>> GetByCustomerOrderShipmentIdAsync(int customerOrderShipmentId, CancellationToken ct)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.CustomerOrderShipmentDetails.AsNoTracking()
                            .Include(i => i.CustomerOrderDetail.ItemReference.Item.Line)
                            .Where(i => i.CustomerOrderShipmentId == customerOrderShipmentId)
                            .ToListAsync(ct);
            }, ct);
        }
    }

}
