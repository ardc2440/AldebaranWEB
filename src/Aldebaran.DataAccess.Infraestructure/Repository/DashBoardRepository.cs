using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class DashBoardRepository : RepositoryBase<AldebaranDbContext>, IDashBoardRepository
    {
        public DashBoardRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<PurchaseOrderDetail>> GetByReferenceIdAndStatusOrderAsync(int statusOrder, int? referenceId = null, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.PurchaseOrderDetails.AsNoTracking()
                            .Include(p => p.PurchaseOrder)
                            .Include(p => p.ItemReference.Item.Line)
                            .Include(p => p.Warehouse)
                            .Where(p => (p.ReferenceId == referenceId || !referenceId.HasValue) && p.PurchaseOrder.StatusDocumentTypeId == statusOrder)
                            .ToListAsync(ct);
            }, ct);
        }

        public async Task<IEnumerable<ItemReference>> GetAllReferencesWithMinimumQuantityAsync(CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.ItemReferences.AsNoTracking()
                            .Include(i => i.Item.Line)
                            .Where(i => i.AlarmMinimumQuantity > 0 && i.InventoryQuantity <= i.AlarmMinimumQuantity && i.IsActive && i.Item.IsActive)
                            .ToListAsync(ct);
            }, ct);
        }
        public async Task<IEnumerable<CustomerReservation>> GetExpiredReservationsAsync(CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.CustomerReservations.AsNoTracking()
                            .Include(i => i.Customer.City.Department.Country)
                            .Include(i => i.Customer.IdentityType)
                            .Include(i => i.StatusDocumentType.DocumentType)
                            .Include(i => i.Employee.IdentityType)
                            .Where(i => i.ExpirationDate.Date <= DateTime.Today && i.StatusDocumentType.StatusOrder == 1)
                            .ToListAsync(ct);
            }, ct);
        }
        public async Task<IEnumerable<Alarm>> GetByEmployeeIdAsync(int employeeId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Alarms.AsNoTracking()
                           .Include(i => i.AlarmMessage.AlarmType.DocumentType)
                           .Where(i => i.ExecutionDate <= DateTime.Now && i.IsActive &&
                                       !dbContext.VisualizedAlarms.AsNoTracking().Any(j => j.AlarmId == i.AlarmId) &&
                                        dbContext.UsersAlarmTypes.AsNoTracking().Any(k => k.Visualize &&
                                                                                         k.EmployeeId == employeeId &&
                                                                                         k.AlarmTypeId == i.AlarmMessage.AlarmTypeId))
                           .ToListAsync(ct);
            }, ct);
        }
        public async Task<StatusDocumentType?> FindByDocumentAndOrderAsync(int documentTypeId, int order, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.StatusDocumentTypes.AsNoTracking().FirstOrDefaultAsync(f => f.DocumentTypeId == documentTypeId && f.StatusOrder == order, ct);
            }, ct);
        }
        public async Task<DocumentType?> FindByCodeAsync(string code, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.DocumentTypes.AsNoTracking().FirstOrDefaultAsync(f => f.DocumentTypeCode == code, ct);
            }, ct);
        }
        public async Task<Employee?> FindByLoginUserIdAsync(string loginUserId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Employees.AsNoTracking()
                            .Include(i => i.Area)
                            .Include(i => i.IdentityType)
                            .FirstOrDefaultAsync(f => f.LoginUserId == loginUserId, ct);
            }, ct);
        }

        public async Task<IEnumerable<PurchaseOrderTransitAlarm>> GetAllTransitAlarmAsync(int employeeId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.PurchaseOrderTransitAlarms.AsNoTracking()
                            .Include(i => i.ModifiedPurchaseOrder.PurchaseOrder.StatusDocumentType)
                            .Include(i => i.ModifiedPurchaseOrder.PurchaseOrder.Provider)
                            .Include(i => i.ModifiedPurchaseOrder.ModificationReason)
                            .Where(w => w.ModifiedPurchaseOrder.PurchaseOrder.StatusDocumentType.StatusOrder == 1 &&
                                        !dbContext.VisualizedPurchaseOrderTransitAlarms.AsNoTracking().Any(j => j.PurchaseOrderTransitAlarmId == w.PurchaseOrderTransitAlarmId &&
                                                                                                               j.EmployeeId == employeeId))
                            .ToListAsync(ct);
            }, ct);
        }

        public async Task<IEnumerable<PurchaseOrder>> GetPurchaseOrderExpirationsAsync(int purchaseOrderWitheFlag, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.PurchaseOrders.AsNoTracking()
                            .Include(i => i.Provider)
                            .Include(i => i.StatusDocumentType)
                            .Include(i => i.ShipmentForwarderAgentMethod.ForwarderAgent.Forwarder)
                            .Include(i => i.ShipmentForwarderAgentMethod.ShipmentMethod)
                            .Where(w => w.StatusDocumentType.StatusOrder == 1 &&
                                        EF.Functions.DateDiffDay(DateTime.Today, w.ExpectedReceiptDate) <= purchaseOrderWitheFlag)
                            .ToListAsync(ct);
            }, ct);
        }

        public async Task<IEnumerable<CustomerOrder>> GetExpiredCustomerOrdersAsync(CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.CustomerOrders.AsNoTracking()
                            .Include(i => i.Customer)
                            .Include(i => i.StatusDocumentType)
                            .Where(i => i.EstimatedDeliveryDate.Date <= DateTime.Today &&
                                        (i.StatusDocumentType.StatusOrder == 1 ||
                                         i.StatusDocumentType.StatusOrder == 2 ||
                                         i.StatusDocumentType.StatusOrder == 3))
                            .ToListAsync(ct);
            }, ct);
        }
    }
}
