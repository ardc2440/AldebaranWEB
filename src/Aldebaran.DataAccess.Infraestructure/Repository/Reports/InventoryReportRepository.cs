using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class InventoryReportRepository : IInventoryReportRepository
    {
        private readonly AldebaranDbContext _context;
        public InventoryReportRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<InventoryReport>> GetInventoryReportDataAsync(string referenceIdsFilter, CancellationToken ct = default)
        {
            return await _context.Set<InventoryReport>().FromSqlRaw($"EXEC SP_GET_INVENTORY_REPORT @ReferenceIds = '{referenceIdsFilter}'").ToListAsync(ct);
        }
    }
}
