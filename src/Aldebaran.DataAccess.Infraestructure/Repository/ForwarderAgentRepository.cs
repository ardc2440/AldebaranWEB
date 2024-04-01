using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ForwarderAgentRepository : IForwarderAgentRepository
    {
        private readonly AldebaranDbContext _context;
        public ForwarderAgentRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<bool> ExistsByAgentName(int forwarderId, string agentName, CancellationToken ct = default)
        {
            return await _context.ForwarderAgents.AsNoTracking().AnyAsync(i => i.ForwarderId == forwarderId && i.ForwarderAgentName.Trim().ToLower() == agentName.Trim().ToLower(), ct);
        }
        public async Task AddAsync(ForwarderAgent forwarderAgent, CancellationToken ct = default)
        {
            await _context.ForwarderAgents.AddAsync(forwarderAgent, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int forwarderAgentId, CancellationToken ct = default)
        {
            var entity = await _context.ForwarderAgents.FirstOrDefaultAsync(x => x.ForwarderAgentId == forwarderAgentId, ct) ?? throw new KeyNotFoundException($"Agente con id {forwarderAgentId} no existe.");
            _context.ForwarderAgents.Remove(entity);
            try
            {
                await _context.SaveChangesAsync(ct);
            }
            catch
            {
                _context.Entry(entity).State = EntityState.Unchanged;
                throw;
            }
        }

        public async Task<ForwarderAgent?> FindAsync(int forwarderAgentId, CancellationToken ct = default)
        {
            return await _context.ForwarderAgents.AsNoTracking()
                .Include(i => i.Forwarder)
                .Include(i => i.City.Department.Country)
                .FirstOrDefaultAsync(w => w.ForwarderAgentId == forwarderAgentId, ct);
        }

        public async Task<IEnumerable<ForwarderAgent>> GetByForwarderIdAsync(int forwarderId, CancellationToken ct = default)
        {
            return await _context.ForwarderAgents.AsNoTracking()
                .Where(w => w.ForwarderId == forwarderId)
                .Include(i => i.Forwarder)
                .Include(i => i.City.Department.Country)
                .ToListAsync(ct);
        }

        public async Task UpdateAsync(int forwarderAgentId, ForwarderAgent forwarderAgent, CancellationToken ct = default)
        {
            var entity = await _context.ForwarderAgents.FirstOrDefaultAsync(x => x.ForwarderAgentId == forwarderAgentId, ct) ?? throw new KeyNotFoundException($"Agente con id {forwarderAgentId} no existe.");
            entity.ForwarderAgentName = forwarderAgent.ForwarderAgentName;
            entity.Phone1 = forwarderAgent.Phone1;
            entity.Phone2 = forwarderAgent.Phone2;
            entity.Fax = forwarderAgent.Fax;
            entity.ForwarderAgentAddress = forwarderAgent.ForwarderAgentAddress;
            entity.CityId = forwarderAgent.CityId;
            entity.Contact = forwarderAgent.Contact;
            entity.Email1 = forwarderAgent.Email1;
            entity.Email2 = forwarderAgent.Email2;
            await _context.SaveChangesAsync(ct);
        }
    }
}
