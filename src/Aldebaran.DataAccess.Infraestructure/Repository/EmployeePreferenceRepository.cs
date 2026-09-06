using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    internal class EmployeePreferenceRepository : RepositoryBase<AldebaranDbContext>, IEmployeePreferenceRepository
    {
        public EmployeePreferenceRepository(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public async Task<EmployeePreference?> FindAsync(int employeeId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.EmployeePreferences
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.EmployeeId == employeeId, ct);
            }, ct);
        }

        public async Task AddAsync(EmployeePreference employeePreference, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                try
                {
                    await dbContext.EmployeePreferences.AddAsync(employeePreference, ct);

                    await dbContext.SaveChangesAsync(ct);
                }
                catch (Exception ex)
                {
                    dbContext.Entry(employeePreference).State = EntityState.Unchanged;
                    throw;
                }
            }, ct);
        }

        public async Task UpdateAsync(EmployeePreference employeePreference, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.EmployeePreferences
                                .FirstOrDefaultAsync(x => x.EmployeeId == employeePreference.EmployeeId, ct) ?? throw new KeyNotFoundException($"EmployeePreference with id {employeePreference.EmployeeId} does not exist.");

                entity.UpdatedDate = employeePreference.UpdatedDate;
                entity.EnableNotifications = employeePreference.EnableNotifications;
                
                try
                {
                    await dbContext.SaveChangesAsync(ct);
                }
                catch
                {
                    dbContext.Entry(entity)
                        .State = EntityState.Unchanged;

                    throw;
                }
            }, ct);           
        }
    }
}