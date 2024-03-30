using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class FreezoneVsAvailableReportRepository : IFreezoneVsAvailableReportRepository
    {
        private readonly AldebaranDbContext _context;
        public FreezoneVsAvailableReportRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<FreezoneVsAvailableReport>> GetFreezoneVsAvailableReportDataAsync(string filter = "", CancellationToken ct = default)
        {
            return await _context.Set<FreezoneVsAvailableReport>().FromSqlRaw($"EXEC SP_GET_FREEZONE_VS_AVAILABLE_REPORT @ReferenceIds = '{filter}'").ToListAsync(ct);
        }
    }
}
