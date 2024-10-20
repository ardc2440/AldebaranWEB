using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class VisualizedMinimumLocalWarehouseQuantityAlarmRepository: RepositoryBase<AldebaranDbContext>, IVisualizedMinimumLocalWarehouseQuantityAlarmRepository
    {
        public VisualizedMinimumLocalWarehouseQuantityAlarmRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task AddAsync(VisualizedMinimumLocalWarehouseQuantityAlarm item, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                try
                {
                    await dbContext.VisualizedMinimumLocalWarehouseQuantityAlarms.AddAsync(item, ct);
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
