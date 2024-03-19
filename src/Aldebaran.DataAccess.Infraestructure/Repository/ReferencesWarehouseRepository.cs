using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ReferencesWarehouseRepository : IReferencesWarehouseRepository
    {
        private readonly AldebaranDbContext _context;
        public ReferencesWarehouseRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<ReferencesWarehouse>> GetAllAsync(CancellationToken ct = default) 
        {
            return await _context.ReferencesWarehouses.AsNoTracking()
                .Include(x => x.ItemReference.Item.Line)
                .Include(x => x.Warehouse)
                .ToListAsync(ct);
        }

        public async Task<ReferencesWarehouse?> GetByReferenceAndWarehouseIdAsync(int referenceId, short warehouseId, CancellationToken ct = default)
        {
            return await _context.ReferencesWarehouses.AsNoTracking()
                .Include(x => x.ItemReference.Item.Line)
                .Include(x => x.Warehouse)
                .FirstOrDefaultAsync(x => x.ReferenceId == referenceId && x.WarehouseId == warehouseId, ct);
        }

        public async Task<IEnumerable<ReferencesWarehouse>> GetByReferenceIdAsync(int referenceId, CancellationToken ct = default)
        {
            return await _context.ReferencesWarehouses.AsNoTracking()
                .Include(x => x.ItemReference.Item.Line)
                .Include(x => x.Warehouse)
                .Where(x => x.ReferenceId == referenceId)
                .ToListAsync(ct);
        }
    }
}
