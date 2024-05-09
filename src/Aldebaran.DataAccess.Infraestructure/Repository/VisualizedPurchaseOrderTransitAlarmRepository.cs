using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class VisualizedPurchaseOrderTransitAlarmRepository: IVisualizedPurchaseOrderTransitAlarmRepository
    {
        private readonly AldebaranDbContext _context;
        public VisualizedPurchaseOrderTransitAlarmRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddAsync(VisualizedPurchaseOrderTransitAlarm item, CancellationToken ct = default)
        {
            try
            {
                await _context.VisualizedPurchaseOrderTransitAlarms.AddAsync(item, ct);
                await _context.SaveChangesAsync(ct);
            }
            catch (Exception)
            {
                _context.Entry(item).State = EntityState.Unchanged;
                throw;
            }            
        }
    }
}
