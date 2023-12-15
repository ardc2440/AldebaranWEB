using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly AldebaranDbContext _context;
        public CurrencyRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Currency>> GetAsync(CancellationToken ct = default)
        {
            return await _context.Currencies.AsNoTracking()
               .ToListAsync(ct);
        }
    }
}
