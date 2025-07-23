using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class VisualizedAutomaticCustomerInProcessModificationRepository : RepositoryBase<AldebaranDbContext>, IVisualizedAutomaticCustomerInProcessModificationRepository
    {
        public VisualizedAutomaticCustomerInProcessModificationRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task AddAsync(VisualizedAutomaticCustomerOrderInProcessModification item, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                try
                {
                    await dbContext.VisualizedAutomaticCustomerOrderInProcessModifications.AddAsync(item, ct);
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
