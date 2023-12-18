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
    }
}
