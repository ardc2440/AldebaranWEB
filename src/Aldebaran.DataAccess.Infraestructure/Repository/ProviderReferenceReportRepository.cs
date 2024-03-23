using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ProviderReferenceReportRepository : IProviderReferenceReportRepository
    {
        private readonly AldebaranDbContext _context;
        public ProviderReferenceReportRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<ProviderReferenceReport>> GetProviderReferenceReportDataAsync(CancellationToken ct)
        {
            return await _context.Set<ProviderReferenceReport>().ToListAsync(ct);
        }
    }
}
