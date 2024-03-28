using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class ProviderReferenceReportRepository : IProviderReferenceReportRepository
    {
        private readonly AldebaranDbContext _context;
        public ProviderReferenceReportRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<ProviderReferenceReport>> GetProviderReferenceReportDataAsync(string filter, CancellationToken ct)
        {
            return await _context.Set<ProviderReferenceReport>().FromSqlRaw($"EXEC SP_GET_PROVIDER_REFERENCE_REPORT {filter}").ToListAsync(ct);
        }
    }
}
