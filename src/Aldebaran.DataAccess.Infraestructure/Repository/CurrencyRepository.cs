using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CurrencyRepository : RepositoryBase<AldebaranDbContext>, ICurrencyRepository
    {
        public CurrencyRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<Currency>> GetAsync(CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Currencies.AsNoTracking()
                           .ToListAsync(ct);
            }, ct);
        }
    }
}
