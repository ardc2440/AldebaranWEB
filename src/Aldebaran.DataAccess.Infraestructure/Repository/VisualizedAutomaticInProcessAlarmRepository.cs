using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class VisualizedAutomaticInProcessAlarmRepository : RepositoryBase<AldebaranDbContext>, IVisualizedAutomaticInProcessAlarmRepository
    {
        public VisualizedAutomaticInProcessAlarmRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task AddAsync(VisualizedAutomaticInProcess item, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                try
                {
                    await dbContext.VisualizedAutomaticInProcesses.AddAsync(item, ct);
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
