using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CountryRepository : ICountryRepository
    {
        private readonly AldebaranDbContext _context;
        public CountryRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<Country?> FindAsync(int countryId, CancellationToken ct = default)
        {
            return await _context.Countries.AsNoTracking()
                .FirstOrDefaultAsync(f => f.CountryId == countryId, ct);
        }

        public async Task<IEnumerable<Country>> GetAsync(CancellationToken ct = default)
        {
            return await _context.Countries.AsNoTracking()
                .ToListAsync(ct);
        }
    }
}
