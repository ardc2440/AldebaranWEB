using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CustomerOrderActivityDetailRepository : RepositoryBase<AldebaranDbContext>, ICustomerOrderActivityDetailRepository
    {
        public CustomerOrderActivityDetailRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task DeleteAsync(int customerOrderActivityDetailId, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.CustomerOrderActivityDetails.FirstOrDefaultAsync(i => i.CustomerOrderActivityDetailId == customerOrderActivityDetailId, ct) ?? throw new KeyNotFoundException($"Detalle de Actividad con id {customerOrderActivityDetailId} no existe."); ;
                try
                {
                    dbContext.CustomerOrderActivityDetails.Remove(entity);
                    await dbContext.SaveChangesAsync(ct);
                }
                catch
                {
                    dbContext.Entry(entity).State = EntityState.Unchanged;
                    throw;
                }
                return Task.CompletedTask;
            }, ct);
        }

        public async Task<IEnumerable<CustomerOrderActivityDetail>> GetByCustomerOrderActivityIdAsync(int customerOrderActivityId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.CustomerOrderActivityDetails.AsNoTracking()
                            .Include(i => i.ActivityType)
                            .Include(i => i.ActivityEmployee)
                            .Include(i => i.Employee_EmployeeId.IdentityType)
                            .Where(i => i.CustomerOrderActivityId.Equals(customerOrderActivityId))
                            .ToListAsync(ct);
            }, ct);
        }
    }

}
