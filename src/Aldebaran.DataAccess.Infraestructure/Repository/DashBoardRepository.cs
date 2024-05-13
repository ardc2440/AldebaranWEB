using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class DashBoardRepository : IDashBoardRepository
    {
        private readonly AldebaranDashBoardDbContext _context;
        public DashBoardRepository(AldebaranDashBoardDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<PurchaseOrderDetail>> GetByReferenceIdAndStatusOrderAsync(int statusOrder, int? referenceId = null, CancellationToken ct = default)
        {
            return await _context.PurchaseOrderDetails.AsNoTracking()
                .Include(p => p.PurchaseOrder)
                .Include(p => p.ItemReference.Item.Line)
                .Include(p => p.Warehouse)
                .Where(p => (p.ReferenceId == referenceId || !referenceId.HasValue) && p.PurchaseOrder.StatusDocumentTypeId == statusOrder)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<ItemReference>> GetAllReferencesWithMinimumQuantityAsync(CancellationToken ct = default)
        {
            return await _context.ItemReferences.AsNoTracking()
                .Include(i => i.Item.Line)
                .Where(i => i.AlarmMinimumQuantity > 0 && i.InventoryQuantity <= i.AlarmMinimumQuantity && i.IsActive && i.Item.IsActive)
                .ToListAsync(ct);
        }
        public async Task<IEnumerable<CustomerReservation>> GetExpiredReservationsAsync(CancellationToken ct = default)
        {
            return await _context.CustomerReservations.AsNoTracking()
                .Include(i => i.Customer.City.Department.Country)
                .Include(i => i.Customer.IdentityType)
                .Include(i => i.StatusDocumentType.DocumentType)
                .Include(i => i.Employee.IdentityType)
                .Where(i => i.ExpirationDate.Date <= DateTime.Today && i.StatusDocumentType.StatusOrder == 1)
                .ToListAsync(ct);
        }
        public async Task<IEnumerable<Alarm>> GetByEmployeeIdAsync(int employeeId, CancellationToken ct = default)
        {
            return await _context.Alarms.AsNoTracking()
                            .Include(i => i.AlarmMessage.AlarmType.DocumentType)
                            .Where(i => i.ExecutionDate <= DateTime.Now && i.IsActive &&
                                        !_context.VisualizedAlarms.AsNoTracking().Any(j => j.AlarmId == i.AlarmId) &&
                                         _context.UsersAlarmTypes.AsNoTracking().Any(k => k.Visualize &&
                                                                                          k.EmployeeId == employeeId &&
                                                                                          k.AlarmTypeId == i.AlarmMessage.AlarmTypeId))
                            .ToListAsync(ct);
        }
        public async Task<StatusDocumentType?> FindByDocumentAndOrderAsync(int documentTypeId, int order, CancellationToken ct = default)
        {
            return await _context.StatusDocumentTypes.AsNoTracking().FirstOrDefaultAsync(f => f.DocumentTypeId == documentTypeId && f.StatusOrder == order, ct);
        }
        public async Task<DocumentType?> FindByCodeAsync(string code, CancellationToken ct = default)
        {
            return await _context.DocumentTypes.AsNoTracking().FirstOrDefaultAsync(f => f.DocumentTypeCode == code, ct);
        }
        public async Task<Employee?> FindByLoginUserIdAsync(string loginUserId, CancellationToken ct = default)
        {
            return await _context.Employees.AsNoTracking()
                .Include(i => i.Area)
                .Include(i => i.IdentityType)
                .FirstOrDefaultAsync(f => f.LoginUserId == loginUserId, ct);
        }

        public async Task<IEnumerable<PurchaseOrderTransitAlarm>> GetAllTransitAlarmAsync(int employeeId, CancellationToken ct = default)
        {
            return await _context.PurchaseOrderTransitAlarms.AsNoTracking()
                .Include(i => i.ModifiedPurchaseOrder.PurchaseOrder.StatusDocumentType)
                .Include(i => i.ModifiedPurchaseOrder.PurchaseOrder.Provider)
                .Include(i => i.ModifiedPurchaseOrder.ModificationReason)
                .Where(w => w.ModifiedPurchaseOrder.PurchaseOrder.StatusDocumentType.StatusOrder == 1 &&
                            !_context.VisualizedPurchaseOrderTransitAlarms.AsNoTracking().Any(j => j.PurchaseOrderTransitAlarmId == w.PurchaseOrderTransitAlarmId &&
                                                                                                   j.EmployeeId == employeeId))
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<PurchaseOrder>> GetPurchaseOrderExpirationsAsync(int purchaseOrderWitheFlag, CancellationToken ct = default)
        {
            return await _context.PurchaseOrders.AsNoTracking()
                .Include(i => i.Provider)
                .Include(i => i.StatusDocumentType)
                .Include(i => i.ShipmentForwarderAgentMethod.ForwarderAgent.Forwarder)
                .Include(i => i.ShipmentForwarderAgentMethod.ShipmentMethod)
                .Where(w => w.StatusDocumentType.StatusOrder == 1 &&
                            EF.Functions.DateDiffDay(DateTime.Today, w.ExpectedReceiptDate) <= purchaseOrderWitheFlag)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<CustomerOrder>> GetExpiredCustomerOrdersAsync(CancellationToken ct = default)
        {
            return await _context.CustomerOrders.AsNoTracking()
                .Include(i => i.Customer)
                .Include(i => i.StatusDocumentType)
                .Where(i => i.EstimatedDeliveryDate.Date <= DateTime.Today && 
                            (i.StatusDocumentType.StatusOrder == 1 || 
                             i.StatusDocumentType.StatusOrder == 2 || 
                             i.StatusDocumentType.StatusOrder == 3))
                .ToListAsync(ct);
        }
    }
}
