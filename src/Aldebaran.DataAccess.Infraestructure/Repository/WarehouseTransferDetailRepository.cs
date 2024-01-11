using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class WarehouseTransferDetailRepository : IWarehouseTransferDetailRepository
    {
        private readonly AldebaranDbContext _context;
        public WarehouseTransferDetailRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<WarehouseTransferDetail>> GetByWarehouseTransferIdAsync(int warehouseTransferId, CancellationToken ct = default)
        {
            return await _context.WarehouseTransferDetails.AsNoTracking()
                .Where(i => i.WarehouseTransferId == warehouseTransferId)
                .ToListAsync(ct);
        }
    }

}
