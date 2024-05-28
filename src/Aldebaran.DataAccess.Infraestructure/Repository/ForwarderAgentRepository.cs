using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ForwarderAgentRepository : RepositoryBase<AldebaranDbContext>, IForwarderAgentRepository
    {
        public ForwarderAgentRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<bool> ExistsByAgentName(int forwarderId, string agentName, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.ForwarderAgents.AsNoTracking().AnyAsync(i => i.ForwarderId == forwarderId && i.ForwarderAgentName.Trim().ToLower() == agentName.Trim().ToLower(), ct);
            }, ct);
        }

        public async Task AddAsync(ForwarderAgent forwarderAgent, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                await dbContext.ForwarderAgents.AddAsync(forwarderAgent, ct);
                await dbContext.SaveChangesAsync(ct);
            }, ct);
        }

        public async Task DeleteAsync(int forwarderAgentId, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.ForwarderAgents.FirstOrDefaultAsync(x => x.ForwarderAgentId == forwarderAgentId, ct) ?? throw new KeyNotFoundException($"Agente con id {forwarderAgentId} no existe.");
                dbContext.ForwarderAgents.Remove(entity);
                try
                {
                    await dbContext.SaveChangesAsync(ct);
                }
                catch
                {
                    dbContext.Entry(entity).State = EntityState.Unchanged;
                    throw;
                }
            }, ct);
        }

        public async Task<ForwarderAgent?> FindAsync(int forwarderAgentId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.ForwarderAgents.AsNoTracking()
                            .Include(i => i.Forwarder)
                            .Include(i => i.City.Department.Country)
                            .FirstOrDefaultAsync(w => w.ForwarderAgentId == forwarderAgentId, ct);
            }, ct);
        }

        public async Task<IEnumerable<ForwarderAgent>> GetByForwarderIdAsync(int forwarderId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.ForwarderAgents.AsNoTracking()
               .Where(w => w.ForwarderId == forwarderId)
               .Include(i => i.Forwarder)
               .Include(i => i.City.Department.Country)
               .ToListAsync(ct);
            }, ct);
        }

        public async Task UpdateAsync(int forwarderAgentId, ForwarderAgent forwarderAgent, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.ForwarderAgents.FirstOrDefaultAsync(x => x.ForwarderAgentId == forwarderAgentId, ct) ?? throw new KeyNotFoundException($"Agente con id {forwarderAgentId} no existe.");
                entity.ForwarderAgentName = forwarderAgent.ForwarderAgentName;
                entity.Phone1 = forwarderAgent.Phone1;
                entity.Phone2 = forwarderAgent.Phone2;
                entity.Fax = forwarderAgent.Fax;
                entity.ForwarderAgentAddress = forwarderAgent.ForwarderAgentAddress;
                entity.CityId = forwarderAgent.CityId;
                entity.Contact = forwarderAgent.Contact;
                entity.Email1 = forwarderAgent.Email1;
                entity.Email2 = forwarderAgent.Email2;
                await dbContext.SaveChangesAsync(ct);
            }, ct);
        }
    }
}
