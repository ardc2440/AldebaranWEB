using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class VisualizedAlarmRepository : RepositoryBase<AldebaranDbContext>, IVisualizedAlarmRepository
    {
        public VisualizedAlarmRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task AddAsync(VisualizedAlarm item, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                try
                {
                    await dbContext.VisualizedAlarms.AddAsync(item, ct);
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
