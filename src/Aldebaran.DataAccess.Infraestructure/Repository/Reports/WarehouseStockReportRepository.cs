using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class WarehouseStockReportRepository : IWarehouseStockReportRepository
    {
        private readonly AldebaranDbContext _context;
        public WarehouseStockReportRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<IEnumerable<WarehouseStockReport>> GetWarehouseStockReportDataAsync(CancellationToken ct = default)
        {
            return await _context.Set<WarehouseStockReport>().ToListAsync(ct);
        }
    }
}
