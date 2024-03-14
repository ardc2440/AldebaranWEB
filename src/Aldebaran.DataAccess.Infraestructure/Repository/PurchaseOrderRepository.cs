using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Infraestructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class PurchaseOrderRepository : IPurchaseOrderRepository
    {
        private readonly AldebaranDbContext _context;
        public PurchaseOrderRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<PurchaseOrder> AddAsync(PurchaseOrder item, CancellationToken ct = default)
        {
            await _context.PurchaseOrders.AddAsync(item, ct);
            await _context.SaveChangesAsync(ct);
            return item;
        }

        public async Task CancelAsync(int purchaseOrderId, Reason reason, CancellationToken ct = default)
        {
            var entity = await _context.PurchaseOrders.FirstOrDefaultAsync(x => x.PurchaseOrderId == purchaseOrderId, ct) ?? throw new KeyNotFoundException($"Orden con id {purchaseOrderId} no existe.");
            var documentType = await _context.DocumentTypes.AsNoTracking().FirstAsync(f => f.DocumentTypeCode == "O", ct);
            var statutsDocumentType = await _context.StatusDocumentTypes.AsNoTracking().FirstAsync(f => f.DocumentTypeId == documentType.DocumentTypeId && f.StatusOrder == 3, ct);
            entity.StatusDocumentTypeId = statutsDocumentType.StatusDocumentTypeId;

            var reasonEntity = new CanceledPurchaseOrder
            {
                PurchaseOrderId = purchaseOrderId,
                CancellationReasonId = reason.ReasonId,
                EmployeeId = reason.EmployeeId,
                CancellationDate = reason.Date
            };
            try
            {
                _context.CanceledPurchaseOrders.Add(reasonEntity);
                await _context.SaveChangesAsync(ct);
            }
            catch
            {
                _context.Entry(entity).State = EntityState.Unchanged;
                _context.Entry(reasonEntity).State = EntityState.Unchanged;
                throw;
            }
        }

        public async Task ConfirmAsync(int purchaseOrderId, PurchaseOrder purchaseOrder, CancellationToken ct = default)
        {
            var entity = await _context.PurchaseOrders.FirstOrDefaultAsync(x => x.PurchaseOrderId == purchaseOrderId, ct) ?? throw new KeyNotFoundException($"Orden con id {purchaseOrderId} no existe.");
            entity.RealReceiptDate = purchaseOrder.RealReceiptDate;
            entity.ImportNumber = purchaseOrder.ImportNumber;
            entity.EmbarkationPort = purchaseOrder.EmbarkationPort;
            entity.ProformaNumber = purchaseOrder.ProformaNumber;

            var documentType = await _context.DocumentTypes.AsNoTracking().FirstAsync(f => f.DocumentTypeCode == "O", ct);
            var statutsDocumentType = await _context.StatusDocumentTypes.AsNoTracking().FirstAsync(f => f.DocumentTypeId == documentType.DocumentTypeId && f.StatusOrder == 2, ct);
            entity.StatusDocumentTypeId = statutsDocumentType.StatusDocumentTypeId;

            // Details
            var details = await _context.PurchaseOrderDetails.Where(x => x.PurchaseOrderId == purchaseOrderId).ToListAsync(ct);
            foreach (var detail in purchaseOrder.PurchaseOrderDetails)
            {
                var detailToUpdate = details.FirstOrDefault(f => f.PurchaseOrderDetailId == detail.PurchaseOrderDetailId);
                if (detailToUpdate != null)
                    detailToUpdate.ReceivedQuantity = detail.ReceivedQuantity;
            }
            try
            {
                await _context.SaveChangesAsync(ct);
            }
            catch
            {
                _context.Entry(entity).State = EntityState.Unchanged;
                throw;
            }
        }

        public async Task<PurchaseOrder?> FindAsync(int purchaseOrderId, CancellationToken ct = default)
        {
            return await _context.PurchaseOrders.AsNoTracking()
               .Include(i => i.Employee.Area)
               .Include(i => i.Employee.IdentityType)
               .Include(i => i.ForwarderAgent.Forwarder)
               .Include(i => i.Provider.IdentityType)
               .Include(i => i.ShipmentForwarderAgentMethod.ShipmentMethod)
               .Include(i => i.ShipmentForwarderAgentMethod.ForwarderAgent)
               .Include(i => i.StatusDocumentType.DocumentType)
               .Where(w => w.PurchaseOrderId == purchaseOrderId)
               .FirstOrDefaultAsync(ct);
        }

        public async Task<IEnumerable<PurchaseOrder>> GetAsync(CancellationToken ct = default)
        {
            return await _context.PurchaseOrders.AsNoTracking()
                .Include(i => i.Employee.Area)
                .Include(i => i.Employee.IdentityType)
                .Include(i => i.ForwarderAgent.Forwarder)
                .Include(i => i.Provider.IdentityType)
                .Include(i => i.ShipmentForwarderAgentMethod.ShipmentMethod)
                .Include(i => i.ShipmentForwarderAgentMethod.ForwarderAgent)
                .Include(i => i.StatusDocumentType.DocumentType)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<PurchaseOrder>> GetAsync(string searchKey, CancellationToken ct = default)
        {
            return await _context.PurchaseOrders.AsNoTracking()
               .Include(i => i.Employee.Area)
               .Include(i => i.Employee.IdentityType)
               .Include(i => i.ForwarderAgent.Forwarder)
               .Include(i => i.Provider.IdentityType)
               .Include(i => i.ShipmentForwarderAgentMethod.ShipmentMethod)
               .Include(i => i.ShipmentForwarderAgentMethod.ForwarderAgent)
               .Include(i => i.StatusDocumentType.DocumentType)
               .Where(w => w.OrderNumber.Contains(searchKey) || w.ImportNumber.Contains(searchKey) || w.EmbarkationPort.Contains(searchKey) || w.ProformaNumber.Contains(searchKey))
               .ToListAsync(ct);
        }

        public async Task<IEnumerable<PurchaseOrder>> GetTransitByReferenceId(int referenceId, CancellationToken ct = default)
        {
            return await _context.PurchaseOrders.AsNoTracking()
               .Include(i => i.PurchaseOrderDetails)
               .Include(i => i.PurchaseOrderActivities)
               .Where(w => w.StatusDocumentType.StatusOrder == 1 && _context.PurchaseOrderDetails.AsNoTracking().Any(d => d.PurchaseOrderId == w.PurchaseOrderId && d.ReferenceId == referenceId))
               .ToListAsync(ct);
        }

        public async Task UpdateAsync(int purchaseOrderId, PurchaseOrder purchaseOrder, Reason reason, CancellationToken ct = default)
        {
            var entity = await _context.PurchaseOrders
                .FirstOrDefaultAsync(x => x.PurchaseOrderId == purchaseOrderId, ct) ?? throw new KeyNotFoundException($"Orden con id {purchaseOrderId} no existe.");
            entity.RequestDate = purchaseOrder.RequestDate;
            entity.ExpectedReceiptDate = purchaseOrder.ExpectedReceiptDate;
            entity.ProviderId = purchaseOrder.ProviderId;
            entity.ForwarderAgentId = purchaseOrder.ForwarderAgentId;
            entity.ShipmentForwarderAgentMethodId = purchaseOrder.ShipmentForwarderAgentMethodId;
            // Details
            var details = await _context.PurchaseOrderDetails.Where(x => x.PurchaseOrderId == purchaseOrderId).ToListAsync(ct);
            _context.PurchaseOrderDetails.RemoveRange(details);
            entity.PurchaseOrderDetails = purchaseOrder.PurchaseOrderDetails;
            // Activities
            var activities = await _context.PurchaseOrderActivities.Where(x => x.PurchaseOrderId == purchaseOrderId).ToListAsync(ct);
            _context.PurchaseOrderActivities.RemoveRange(activities);
            entity.PurchaseOrderActivities = purchaseOrder.PurchaseOrderActivities;

            var reasonEntity = new ModifiedPurchaseOrder
            {
                PurchaseOrderId = purchaseOrderId,
                ModificationReasonId = reason.ReasonId,
                EmployeeId = reason.EmployeeId,
                ModificationDate = reason.Date
            };
            try
            {
                _context.ModifiedPurchaseOrders.Add(reasonEntity);
                await _context.SaveChangesAsync(ct);
            }
            catch
            {
                _context.Entry(entity).State = EntityState.Unchanged;
                _context.Entry(reasonEntity).State = EntityState.Unchanged;
                throw;
            }
        }
    }
}
