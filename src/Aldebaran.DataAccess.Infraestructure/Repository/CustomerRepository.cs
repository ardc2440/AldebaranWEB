using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AldebaranDbContext _context;
        public CustomerRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Customer?> FindAsync(int customerId, CancellationToken ct = default)
        {
            return await _context.Customers
                .AsNoTracking()
                .Include(i => i.IdentityType)
                .Include(i => i.City.Department.Country)
                .FirstOrDefaultAsync(i => i.CustomerId == customerId);

        }
        public async Task<bool> ExistsByName(string name, CancellationToken ct = default)
        {
            return await _context.Customers.AsNoTracking().AnyAsync(i => i.CustomerName.Trim().ToLower() == name.Trim().ToLower(), ct);
        }
        public async Task<bool> ExistsByIdentificationNumber(string identificationNumber, CancellationToken ct = default)
        {
            return await _context.Customers.AsNoTracking().AnyAsync(i => i.IdentityNumber.Trim().ToLower() == identificationNumber.Trim().ToLower(), ct);
        }
        public async Task<IEnumerable<Customer>> GetAsync(CancellationToken ct = default)
        {
            return await _context.Customers
                .AsNoTracking()
                .Include(i => i.IdentityType)
                .Include(i => i.City.Department.Country)
                .ToListAsync();
        }

        public async Task<IEnumerable<Customer>> GetAsync(string searchKey, CancellationToken ct = default)
        {
            return await _context.Customers
                .AsNoTracking()
                .Include(i => i.IdentityType)
                .Include(i => i.City.Department.Country)
                .Where(i => i.IdentityType.IdentityTypeName.Equals(searchKey) ||
                            i.IdentityNumber.Equals(searchKey) ||
                            i.City.Department.Country.CountryName.Equals(searchKey) ||
                            i.City.Department.Country.CountryCode.Equals(searchKey) ||
                            i.City.Department.DepartmentName.Equals(searchKey) ||
                            i.City.CityName.Equals(searchKey) ||
                            i.CustomerAddress.Contains(searchKey) ||
                            i.CustomerName.Contains(searchKey) ||
                            i.Email1.Contains(searchKey) ||
                            i.Email2.Contains(searchKey) ||
                            i.Email3.Contains(searchKey) ||
                            i.Fax.Contains(searchKey) ||
                            i.Phone1.Contains(searchKey) ||
                            i.Phone2.Contains(searchKey))
                .ToListAsync();
        }

        public async Task AddAsync(Customer customer, CancellationToken ct = default)
        {
            await _context.Customers.AddAsync(customer, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(int customerId, Customer customer, CancellationToken ct = default)
        {
            var entity = await _context.Customers.FirstOrDefaultAsync(x => x.CustomerId == customerId, ct) ?? throw new KeyNotFoundException($"Cliente con id {customerId} no existe.");
            entity.CustomerName = customer.CustomerName;
            entity.Phone1 = customer.Phone1;
            entity.Phone2 = customer.Phone2;
            entity.Fax = customer.Fax;
            entity.CustomerAddress = customer.CustomerAddress;
            entity.Email1 = customer.Email1;
            entity.Email2 = customer.Email2;
            entity.Email3 = customer.Email3;
            entity.CityId = customer.CityId;
            entity.IdentityNumber = customer.IdentityNumber;
            entity.CellPhone = customer.CellPhone;
            entity.IdentityTypeId = customer.IdentityTypeId;

            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int customerId, CancellationToken ct = default)
        {
            var entity = await _context.Customers.FirstOrDefaultAsync(x => x.CustomerId == customerId, ct) ?? throw new KeyNotFoundException($"Cliente con id {customerId} no existe.");
            _context.Customers.Remove(entity);
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
    }
}
