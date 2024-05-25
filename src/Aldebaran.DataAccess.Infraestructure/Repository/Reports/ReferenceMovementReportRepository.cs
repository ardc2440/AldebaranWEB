using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class ReferenceMovementReportRepository : IReferenceMovementReportRepository
    {
        private readonly AldebaranDbContext _context;
        public ReferenceMovementReportRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<ReferenceMovementReport>> GetReferenceMovementReportDataAsync(string filter, CancellationToken ct = default)
        {
            return await _context.Set<ReferenceMovementReport>().FromSqlRaw($"EXEC SP_GET_REFERENCE_MOVEMENT_REPORT {filter}").ToListAsync(ct);
        }
    }
}
