using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ShippingMethodRepository : IShippingMethodRepository
    {
        private readonly AldebaranDbContext _context;
        public ShippingMethodRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<ShippingMethod>> GetAsync(CancellationToken ct = default)
        {
            return await _context.ShippingMethods.AsNoTracking().ToListAsync(ct);
        }

        public async Task<ShippingMethod?> FindAsync(short ShippingMethodId, CancellationToken ct = default)
        {
            return await _context.ShippingMethods.AsNoTracking()
                .FirstOrDefaultAsync(i => i.ShippingMethodId == ShippingMethodId, ct);
        }
    }
}
