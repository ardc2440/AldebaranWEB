using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CustomerContactRepository : ICustomerContactRepository
    {
        private readonly AldebaranDbContext _context;
        public CustomerContactRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddAsync(CustomerContact customerContact, CancellationToken ct = default)
        {
            await _context.CustomerContacts.AddAsync(customerContact, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(int customerContactId, CustomerContact customerContact, CancellationToken ct = default)
        {
            var entity = await _context.CustomerContacts.FirstOrDefaultAsync(x => x.CustomerContactId == customerContactId, ct) ?? throw new KeyNotFoundException($"Contacto con id {customerContactId} no existe.");
            entity.Title = customerContact.Title;
            entity.Phone = customerContact.Phone;
            entity.Email = customerContact.Email;
            entity.CustomerContactName = customerContact.CustomerContactName;
            entity.CustomerId = customerContact.CustomerId;

            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int customerContactId, CancellationToken ct = default)
        {
            var entity = await _context.CustomerContacts.FirstOrDefaultAsync(x => x.CustomerContactId == customerContactId, ct) ?? throw new KeyNotFoundException($"Contacto con id {customerContactId} no existe.");
            _context.CustomerContacts.Remove(entity);
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

        public async Task<IEnumerable<CustomerContact>> GetByCustomerIdAsync(int customerId, CancellationToken ct = default)
        {
            return await _context.CustomerContacts.AsNoTracking()
                .Where(x => x.CustomerId == customerId)
                .ToListAsync(ct);
        }

        public async Task<CustomerContact?> FindAsync(int customerContactId, CancellationToken ct = default)
        {
            return await _context.CustomerContacts.AsNoTracking()
                .FirstOrDefaultAsync(i => i.CustomerContactId == customerContactId, ct);
        }
    }

}
