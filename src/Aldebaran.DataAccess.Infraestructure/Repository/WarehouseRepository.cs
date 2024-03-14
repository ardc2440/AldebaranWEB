using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly AldebaranDbContext _context;
        public WarehouseRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<Warehouse>> GetAsync(CancellationToken ct = default)
        {
            return await _context.Warehouses.AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<List<Warehouse>> GetAsync(string searchKey, CancellationToken ct = default)
        {
            return await _context.Warehouses.AsNoTracking()
                .Where(i => i.WarehouseName.Contains(searchKey))
                .ToListAsync(ct);
        }

        public async Task<Warehouse?> FindAsync(short warehouseId, CancellationToken ct = default)
        {
            return await _context.Warehouses.AsNoTracking()
                .FirstOrDefaultAsync(i => i.WarehouseId == warehouseId);
        }

        public async Task<Warehouse?> FindByCodeAsync(short warehouseCode, CancellationToken ct = default)
        {
            return await _context.Warehouses.AsNoTracking()
                .FirstOrDefaultAsync(i => i.WarehouseCode == warehouseCode);
        }
    }

}
