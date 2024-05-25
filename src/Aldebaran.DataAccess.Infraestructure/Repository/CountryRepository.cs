using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CountryRepository : RepositoryBase<AldebaranDbContext>, ICountryRepository
    {
        public CountryRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        public async Task<Country?> FindAsync(int countryId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Countries.AsNoTracking()
                    .FirstOrDefaultAsync(f => f.CountryId == countryId, ct);
            }, ct);
        }

        public async Task<IEnumerable<Country>> GetAsync(CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Countries.AsNoTracking()
                    .ToListAsync(ct);
            }, ct);
        }
    }
}
