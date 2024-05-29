using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class PurchaseOrderTransitAlarmRepository : RepositoryBase<AldebaranDbContext>, IPurchaseOrderTransitAlarmRepository
    {
        public PurchaseOrderTransitAlarmRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task AddAsync(PurchaseOrderTransitAlarm item, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                try
                {
                    await dbContext.PurchaseOrderTransitAlarms.AddAsync(item, ct);
                    await dbContext.SaveChangesAsync(ct);
                }
                catch (Exception)
                {
                    dbContext.Entry(item).State = EntityState.Unchanged;
                    throw;
                }
            }, ct);
        }
    }
}
