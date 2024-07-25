using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CustomerRepository : RepositoryBase<AldebaranDbContext>, ICustomerRepository
    {
        public CustomerRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<Customer?> FindAsync(int customerId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Customers
                            .AsNoTracking()
                            .Include(i => i.IdentityType)
                            .Include(i => i.City.Department.Country)
                            .FirstOrDefaultAsync(i => i.CustomerId == customerId);
            }, ct);
        }
        public async Task<bool> ExistsByName(string name, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Customers.AsNoTracking().AnyAsync(i => i.CustomerName.Trim().ToLower() == name.Trim().ToLower(), ct);
            }, ct);
        }
        public async Task<bool> ExistsByIdentificationNumber(string identificationNumber, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Customers.AsNoTracking().AnyAsync(i => i.IdentityNumber.Trim().ToLower() == identificationNumber.Trim().ToLower(), ct);
            }, ct);
        }
        public async Task<(IEnumerable<Customer>, int)> GetAsync(int? skip=null, int? top= null, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                var a = dbContext.Customers
                            .AsNoTracking()
                            .Include(i => i.IdentityType)
                            .Include(i => i.City.Department.Country)
                            .OrderBy(o=>o.CustomerName);

                if (skip != null && top != null)
                    return (await a.Skip(skip.Value).Take(top.Value).ToListAsync(), await a.CountAsync(ct));

                return (await a.ToListAsync(), await a.CountAsync(ct));

            }, ct);
        }

        public async Task<(IEnumerable<Customer>, int)> GetAsync(int skip, int top, string searchKey, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                var a = dbContext.Customers
                       .AsNoTracking()
                       .Include(i => i.IdentityType)
                       .Include(i => i.City.Department.Country)
                       .Where(i => i.IdentityNumber.Contains(searchKey) ||
                                   i.CustomerName.Contains(searchKey))
                       .OrderBy(o => o.CustomerName); 

                return (await a.Skip(skip).Take(top).ToListAsync(), await a.CountAsync(ct));
            }, ct);
        }

        public async Task AddAsync(Customer customer, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                await dbContext.Customers.AddAsync(customer, ct);
                await dbContext.SaveChangesAsync(ct);
            }, ct);
        }

        public async Task UpdateAsync(int customerId, Customer customer, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.Customers.FirstOrDefaultAsync(x => x.CustomerId == customerId, ct) ?? throw new KeyNotFoundException($"Cliente con id {customerId} no existe.");
                entity.CustomerName = customer.CustomerName;
                entity.Phone1 = customer.Phone1;
                entity.Phone2 = customer.Phone2;
                entity.Fax = customer.Fax;
                entity.CustomerAddress = customer.CustomerAddress;
                entity.Email = customer.Email;
                entity.CityId = customer.CityId;
                entity.IdentityNumber = customer.IdentityNumber;
                entity.CellPhone = customer.CellPhone;
                entity.IdentityTypeId = customer.IdentityTypeId;

                await dbContext.SaveChangesAsync(ct);
            }, ct);
        }

        public async Task DeleteAsync(int customerId, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.Customers.FirstOrDefaultAsync(x => x.CustomerId == customerId, ct) ?? throw new KeyNotFoundException($"Cliente con id {customerId} no existe.");
                dbContext.Customers.Remove(entity);
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
    }
}
