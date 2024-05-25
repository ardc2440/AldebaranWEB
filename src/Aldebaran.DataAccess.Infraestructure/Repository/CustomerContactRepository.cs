using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CustomerContactRepository : RepositoryBase<AldebaranDbContext>, ICustomerContactRepository
    {
        public CustomerContactRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        public async Task<bool> ExistsByContactName(int customerId, string contactName, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.CustomerContacts.AsNoTracking().AnyAsync(i => i.CustomerId == customerId && i.CustomerContactName.Trim().ToLower() == contactName.Trim().ToLower(), ct);
            }, ct);
        }
        public async Task AddAsync(CustomerContact customerContact, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                await dbContext.CustomerContacts.AddAsync(customerContact, ct);
                await dbContext.SaveChangesAsync(ct);
                return Task.CompletedTask;
            }, ct);
        }

        public async Task UpdateAsync(int customerContactId, CustomerContact customerContact, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.CustomerContacts.FirstOrDefaultAsync(x => x.CustomerContactId == customerContactId, ct) ?? throw new KeyNotFoundException($"Contacto con id {customerContactId} no existe.");
                entity.Title = customerContact.Title;
                entity.Phone = customerContact.Phone;
                entity.Email = customerContact.Email;
                entity.CustomerContactName = customerContact.CustomerContactName;
                entity.CustomerId = customerContact.CustomerId;

                await dbContext.SaveChangesAsync(ct);
                return Task.CompletedTask;
            }, ct);
        }

        public async Task DeleteAsync(int customerContactId, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.CustomerContacts.FirstOrDefaultAsync(x => x.CustomerContactId == customerContactId, ct) ?? throw new KeyNotFoundException($"Contacto con id {customerContactId} no existe.");
                dbContext.CustomerContacts.Remove(entity);
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

        public async Task<IEnumerable<CustomerContact>> GetByCustomerIdAsync(int customerId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.CustomerContacts.AsNoTracking()
                            .Where(x => x.CustomerId == customerId)
                            .ToListAsync(ct);
            }, ct);
        }

        public async Task<CustomerContact?> FindAsync(int customerContactId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.CustomerContacts.AsNoTracking()
               .FirstOrDefaultAsync(i => i.CustomerContactId == customerContactId, ct);
            }, ct);
        }
    }

}
