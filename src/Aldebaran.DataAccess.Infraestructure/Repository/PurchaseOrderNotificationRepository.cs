using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class PurchaseOrderNotificationRepository : IPurchaseOrderNotificationRepository
    {
        private readonly AldebaranDbContext _context;
        public PurchaseOrderNotificationRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<PurchaseOrderNotification>> GetByPurchaseOrderId(int purchaseOrderId, CancellationToken ct=default)
        {
            return await _context.PurchaseOrderNotifications.AsNoTracking()
                            .Include(i=>i.ModifiedPurchaseOrder.ModificationReason)
                            .Include(i=>i.CustomerOrder.Customer)
                            .Where(w=>w.ModifiedPurchaseOrder.PurchaseOrderId == purchaseOrderId)
                            .ToListAsync(ct);            
        }
    }
}
