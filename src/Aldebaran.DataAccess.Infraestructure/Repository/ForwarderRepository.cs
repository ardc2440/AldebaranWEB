using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ForwarderRepository : RepositoryBase<AldebaranDbContext>, IForwarderRepository
    {
        public ForwarderRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<bool> ExistsByForwarderName(string forwarderName, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Forwarders.AsNoTracking().AnyAsync(i => i.ForwarderName.Trim().ToLower() == forwarderName.Trim().ToLower(), ct);
            }, ct);
        }

        public async Task AddAsync(Forwarder forwarder, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                await dbContext.Forwarders.AddAsync(forwarder, ct);
                await dbContext.SaveChangesAsync(ct);
                return Task.CompletedTask;
            }, ct);
        }

        public async Task DeleteAsync(int forwarderId, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.Forwarders.FirstOrDefaultAsync(x => x.ForwarderId == forwarderId, ct) ?? throw new KeyNotFoundException($"Transportadora con id {forwarderId} no existe.");
                dbContext.Forwarders.Remove(entity);
                try
                {
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

        public async Task<Forwarder?> FindAsync(int forwarderId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Forwarders.AsNoTracking()
                            .Include(i => i.City.Department.Country)
                            .FirstOrDefaultAsync(w => w.ForwarderId == forwarderId, ct);
            }, ct);
        }

        public async Task<IEnumerable<Forwarder>> GetAsync(CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Forwarders.AsNoTracking()
                            .Include(i => i.City.Department.Country)
                            .ToListAsync(ct);
            }, ct);
        }

        public async Task<IEnumerable<Forwarder>> GetAsync(string searchKey, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Forwarders.AsNoTracking()
                            .Where(w => w.ForwarderName.Contains(searchKey) || w.Phone1.Contains(searchKey) || w.Phone2.Contains(searchKey) || w.Fax.Contains(searchKey) || w.ForwarderAddress.Contains(searchKey) || w.Mail1.Contains(searchKey) || w.Mail2.Contains(searchKey))
                            .Include(i => i.City.Department.Country)
                            .ToListAsync(ct);
            }, ct);
        }

        public async Task UpdateAsync(int forwarderId, Forwarder forwarder, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.Forwarders.FirstOrDefaultAsync(x => x.ForwarderId == forwarderId, ct) ?? throw new KeyNotFoundException($"Transportadora con id {forwarderId} no existe.");
                entity.ForwarderName = forwarder.ForwarderName;
                entity.Phone1 = forwarder.Phone1;
                entity.Phone2 = forwarder.Phone2;
                entity.Fax = forwarder.Fax;
                entity.ForwarderAddress = forwarder.ForwarderAddress;
                entity.Mail1 = forwarder.Mail1;
                entity.Mail2 = forwarder.Mail2;
                entity.CityId = forwarder.CityId;
                await dbContext.SaveChangesAsync(ct);
                return Task.CompletedTask;
            }, ct);
        }
    }
}
