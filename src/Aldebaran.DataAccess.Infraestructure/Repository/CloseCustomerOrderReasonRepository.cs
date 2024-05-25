using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CloseCustomerOrderReasonRepository : RepositoryBase<AldebaranDbContext>, ICloseCustomerOrderReasonRepository
    {
        public CloseCustomerOrderReasonRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        public async Task<IEnumerable<CloseCustomerOrderReason>> GetAsync(CancellationToken ct = default)
        {

            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.CloseCustomerOrderReasons.ToListAsync(ct);
            }, ct);
        }
    }

}
