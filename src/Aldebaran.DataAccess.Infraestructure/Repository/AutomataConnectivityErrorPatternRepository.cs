using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class AutomataConnectivityErrorPatternRepository : RepositoryBase<AldebaranDbContext>, IAutomataConnectivityErrorPatternRepository
    {
        public AutomataConnectivityErrorPatternRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<AutomataConnectivityErrorPattern> CreateAsync(AutomataConnectivityErrorPattern entity, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                await dbContext.AutomataConnectivityErrorPatterns.AddAsync(entity, ct);
                await dbContext.SaveChangesAsync(ct);
            }, ct);
            return entity;
        }

        public async Task<IEnumerable<AutomataConnectivityErrorPattern>> GetAllAsync(CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.AutomataConnectivityErrorPatterns.AsNoTracking().ToListAsync(ct);
            }, ct);
        }

        public async Task<AutomataConnectivityErrorPattern> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.AutomataConnectivityErrorPatterns.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
            }, ct) ?? throw new KeyNotFoundException($"AutomataConnectivityErrorPattern with id {id} not found");
            return entity;
        }

        public async Task<AutomataConnectivityErrorPattern> UpdateAsync(AutomataConnectivityErrorPattern entity, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                dbContext.AutomataConnectivityErrorPatterns.Update(entity);
                await dbContext.SaveChangesAsync(ct);
            }, ct);
            return entity;
        }
    }
}
