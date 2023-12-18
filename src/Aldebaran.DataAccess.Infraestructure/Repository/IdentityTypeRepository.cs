using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class IdentityTypeRepository : IIdentityTypeRepository
    {
        private readonly AldebaranDbContext _context;
        public IdentityTypeRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<IdentityType>> GetAsync(CancellationToken ct = default)
        {
            return await _context.IdentityTypes.AsNoTracking()
             .ToListAsync(ct);
        }
    }
}
