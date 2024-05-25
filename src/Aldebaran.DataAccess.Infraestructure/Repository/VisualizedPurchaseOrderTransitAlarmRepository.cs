using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class VisualizedPurchaseOrderTransitAlarmRepository : RepositoryBase<AldebaranDbContext>, IVisualizedPurchaseOrderTransitAlarmRepository
    {
        public VisualizedPurchaseOrderTransitAlarmRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task AddAsync(VisualizedPurchaseOrderTransitAlarm item, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                try
                {
                    await dbContext.VisualizedPurchaseOrderTransitAlarms.AddAsync(item, ct);
                    await dbContext.SaveChangesAsync(ct);
                }
                catch (Exception)
                {
                    dbContext.Entry(item).State = EntityState.Unchanged;
                    throw;
                }
                return Task.CompletedTask;
            }, ct);
        }
    }
}
