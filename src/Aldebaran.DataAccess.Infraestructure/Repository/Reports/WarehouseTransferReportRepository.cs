using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class WarehouseTransferReportRepository : IWarehouseTransferReportRepository
    {
        private readonly AldebaranDbContext _context;
        public WarehouseTransferReportRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<WarehouseTransferReport>> GetWarehouseTransferReportDataAsync(string filter = "", CancellationToken ct = default)
        {
            return await _context.Set<WarehouseTransferReport>().FromSqlRaw($"EXEC SP_GET_WAREHOUSE_TRANSFER_REPORT {filter}").ToListAsync(ct);
        }
    }
}
