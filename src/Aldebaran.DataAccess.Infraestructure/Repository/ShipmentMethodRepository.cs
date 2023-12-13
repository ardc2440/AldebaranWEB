using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ShipmentMethodRepository : IShipmentMethodRepository
    {
        private readonly AldebaranDbContext _context;
        public ShipmentMethodRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<IEnumerable<ShipmentMethod>> GetAsync(CancellationToken ct = default)
        {
            return await _context.ShipmentMethods.ToListAsync(ct);
        }
    }
}
