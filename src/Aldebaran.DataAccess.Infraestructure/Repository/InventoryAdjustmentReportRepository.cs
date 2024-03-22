using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class InventoryAdjustmentReportRepository : IInventoryAdjustmentReportRepository
    {
        private readonly AldebaranDbContext _context;
        public InventoryAdjustmentReportRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        async Task<IEnumerable<InventoryAdjustmentReport>> IInventoryAdjustmentReportRepository.GetInventoryAdjustmentReportDataAsync(CancellationToken ct = default)
        {
            return await _context.Set<InventoryAdjustmentReport>().ToListAsync(ct);            
        }
    }
}
