using Aldebaran.DataAccess.Entities;
using Aldebaran.Infraestructure.Common.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class DashBoardRepository : RepositoryBase<AldebaranDbContext>, IDashBoardRepository
    {
        private readonly ISharedStringLocalizer _SharedLocalizer;

        public DashBoardRepository(IServiceProvider serviceProvider, ISharedStringLocalizer sharedLocalizer) : base(serviceProvider)
        {
            _SharedLocalizer = sharedLocalizer ?? throw new ArgumentNullException(nameof(ISharedStringLocalizer));
        }

        public async Task<IEnumerable<CustomerReservation>> GetExpiredReservationsAsync(string? searchKey = null, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await (string.IsNullOrEmpty(searchKey) ?
                                dbContext.CustomerReservations.AsNoTracking()
                                    .Include(i => i.Customer.City.Department.Country)
                                    .Include(i => i.Customer.IdentityType)
                                    .Include(i => i.StatusDocumentType.DocumentType)
                                    .Include(i => i.Employee.IdentityType)
                                    .Where(i => i.ExpirationDate.Date == DateTime.Today && i.StatusDocumentType.StatusOrder == 4)
                                    .ToListAsync(ct) :
                                dbContext.CustomerReservations.AsNoTracking()
                                    .Include(i => i.Customer.City.Department.Country)
                                    .Include(i => i.Customer.IdentityType)
                                    .Include(i => i.StatusDocumentType.DocumentType)
                                    .Include(i => i.Employee.IdentityType)
                                    .Where(i => i.ExpirationDate.Date <= DateTime.Today &&
                                                i.StatusDocumentType.StatusOrder == 1 &&
                                                (i.Customer.CustomerName.Contains(searchKey) ||
                                                 i.ReservationNumber.Contains(searchKey) ||
                                                 dbContext.Format(i.ReservationDate, _SharedLocalizer["date:format"]).Contains(searchKey) ||
                                                 dbContext.Format(i.ExpirationDate, _SharedLocalizer["date:format"]).Contains(searchKey)))
                                    .ToListAsync(ct));
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
        public async Task<IEnumerable<PurchaseOrderTransitAlarm>> GetAllTransitAlarmAsync(int employeeId, string? searchKey = null, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await (string.IsNullOrEmpty(searchKey) ?
                                dbContext.PurchaseOrderTransitAlarms.AsNoTracking()
                                    .Include(i => i.ModifiedPurchaseOrder.PurchaseOrder.StatusDocumentType)
                                    .Include(i => i.ModifiedPurchaseOrder.PurchaseOrder.Provider)
                                    .Include(i => i.ModifiedPurchaseOrder.ModificationReason)
                                    .Where(w => w.ModifiedPurchaseOrder.PurchaseOrder.StatusDocumentType.StatusOrder == 1 &&
                                                !dbContext.VisualizedPurchaseOrderTransitAlarms.AsNoTracking().Any(j => j.PurchaseOrderTransitAlarmId == w.PurchaseOrderTransitAlarmId &&
                                                                                                                       j.EmployeeId == employeeId))
                                    .ToListAsync(ct) :
                                dbContext.PurchaseOrderTransitAlarms.AsNoTracking()
                                    .Include(i => i.ModifiedPurchaseOrder.PurchaseOrder.StatusDocumentType)
                                    .Include(i => i.ModifiedPurchaseOrder.PurchaseOrder.Provider)
                                    .Include(i => i.ModifiedPurchaseOrder.ModificationReason)
                                    .Where(w => w.ModifiedPurchaseOrder.PurchaseOrder.StatusDocumentType.StatusOrder == 1 &&
                                                !dbContext.VisualizedPurchaseOrderTransitAlarms.AsNoTracking().Any(j => j.PurchaseOrderTransitAlarmId == w.PurchaseOrderTransitAlarmId &&
                                                                                                                       j.EmployeeId == employeeId) &&
                                                (w.ModifiedPurchaseOrder.PurchaseOrder.OrderNumber.Contains(searchKey) ||
                                                 dbContext.Format(w.OldExpectedReceiptDate, _SharedLocalizer["date:format"]).Contains(searchKey) ||
                                                 dbContext.Format(w.ModifiedPurchaseOrder.PurchaseOrder.ExpectedReceiptDate, _SharedLocalizer["date:format"]).Contains(searchKey) ||
                                                 w.ModifiedPurchaseOrder.PurchaseOrder.Provider.ProviderName.Contains(searchKey) ||
                                                 dbContext.Format(w.ModifiedPurchaseOrder.ModificationDate, _SharedLocalizer["date:format"]).Contains(searchKey) ||
                                                 w.ModifiedPurchaseOrder.ModificationReason.ModificationReasonName.Contains(searchKey)))
                                    .ToListAsync(ct));
            }, ct);
        }
        public async Task<IEnumerable<PurchaseOrder>> GetPurchaseOrderExpirationsAsync(int purchaseOrderWitheFlag, string? searchKey = null, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await (string.IsNullOrEmpty(searchKey) ?
                                dbContext.PurchaseOrders.AsNoTracking()
                                    .Include(i => i.Provider)
                                    .Include(i => i.StatusDocumentType)
                                    .Include(i => i.ShipmentForwarderAgentMethod.ForwarderAgent.Forwarder)
                                    .Include(i => i.ShipmentForwarderAgentMethod.ShipmentMethod)
                                    .Where(w => w.StatusDocumentType.StatusOrder == 1 &&
                                                EF.Functions.DateDiffDay(DateTime.Today, w.ExpectedReceiptDate) <= purchaseOrderWitheFlag)
                                    .ToListAsync(ct) :
                                dbContext.PurchaseOrders.AsNoTracking()
                                    .Include(i => i.Provider)
                                    .Include(i => i.StatusDocumentType)
                                    .Include(i => i.ShipmentForwarderAgentMethod.ForwarderAgent.Forwarder)
                                    .Include(i => i.ShipmentForwarderAgentMethod.ShipmentMethod)
                                    .Where(w => w.StatusDocumentType.StatusOrder == 1 &&
                                                EF.Functions.DateDiffDay(DateTime.Today, w.ExpectedReceiptDate) <= purchaseOrderWitheFlag &&
                                                (w.OrderNumber.Contains(searchKey) ||
                                                 dbContext.Format(w.ExpectedReceiptDate, _SharedLocalizer["date:format"]).Contains(searchKey) ||
                                                 w.Provider.ProviderName.Contains(searchKey) ||
                                                 w.ShipmentForwarderAgentMethod.ForwarderAgent.Forwarder.ForwarderName.Contains(searchKey) ||
                                                 w.ShipmentForwarderAgentMethod.ForwarderAgent.ForwarderAgentName.Contains(searchKey) ||
                                                 w.ShipmentForwarderAgentMethod.ShipmentMethod.ShipmentMethodName.Contains(searchKey)))
                                    .ToListAsync(ct));
            }, ct);
        }
        public async Task<IEnumerable<CustomerOrder>> GetExpiredCustomerOrdersAsync(string? searchKey = null, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await (string.IsNullOrEmpty(searchKey) ?
                                dbContext.CustomerOrders.AsNoTracking()
                                    .Include(i => i.Customer)
                                    .Include(i => i.StatusDocumentType)
                                    .Where(i => i.EstimatedDeliveryDate.Date <= DateTime.Today &&
                                                (i.StatusDocumentType.StatusOrder == 1 ||
                                                 i.StatusDocumentType.StatusOrder == 2 ||
                                                 i.StatusDocumentType.StatusOrder == 3))
                                    .ToListAsync(ct) :
                                dbContext.CustomerOrders.AsNoTracking()
                                    .Include(i => i.Customer)
                                    .Include(i => i.StatusDocumentType)
                                    .Where(i => i.EstimatedDeliveryDate.Date <= DateTime.Today &&
                                                (i.StatusDocumentType.StatusOrder == 1 ||
                                                 i.StatusDocumentType.StatusOrder == 2 ||
                                                 i.StatusDocumentType.StatusOrder == 3) &&
                                                (i.Customer.CustomerName.Contains(searchKey) ||
                                                 i.OrderNumber.Contains(searchKey) ||
                                                 dbContext.Format(i.OrderDate, _SharedLocalizer["date:format"]).Contains(searchKey) ||
                                                 dbContext.Format(i.EstimatedDeliveryDate, _SharedLocalizer["date:format"]).Contains(searchKey) ||
                                                 i.StatusDocumentType.StatusDocumentTypeName.Contains(searchKey)))
                                    .ToListAsync(ct));
            }, ct);
        }
        public async Task<IEnumerable<NotificationWithError>> GetNotificationsWithError(string? searchKey = null, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                if (searchKey.IsNullOrEmpty())
                    return await dbContext.Set<NotificationWithError>()
                    .FromSqlRaw($"EXEC SP_GET_NOTFICATIONS_WITH_SEND_ERROR").ToListAsync(ct);
                else
                {
                    var search = new SqlParameter("@SEARCHKEY", searchKey);

                    return await dbContext.Set<NotificationWithError>()
                        .FromSqlRaw($"EXEC SP_GET_NOTFICATIONS_WITH_SEND_ERROR " +
                        $"@SEARCHKEY",
                        search).ToListAsync(ct);
                }
            }, ct);
        }
        public async Task<IEnumerable<OutOfStockArticle>> GetOutOfStockAlarmsAsync(int employeeId, string? searchKey = null, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                var employee_Id = new SqlParameter("@EmployeeId", employeeId);

                if (searchKey.IsNullOrEmpty())
                    return await dbContext.Set<OutOfStockArticle>()
                        .FromSqlRaw($"EXEC SP_GET_OUT_OF_STOCK_INVENTORY_ALARMS " +
                        $"@EmployeeId",
                        employee_Id).ToListAsync(ct);
                else
                {
                    var search = new SqlParameter("@SEARCHKEY", searchKey);

                    return await dbContext.Set<OutOfStockArticle>()
                        .FromSqlRaw($"EXEC SP_GET_OUT_OF_STOCK_INVENTORY_ALARMS " +
                        $"@EmployeeId, " +
                        $"@SEARCHKEY ",
                        employee_Id, search).ToListAsync(ct);
                }
            }, ct);
        }
        public async Task<IEnumerable<MinimumQuantityArticle>> GetMinimumQuantityAlarmsAsync(int employeeId, string? searchKey = null, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                var employee_Id = new SqlParameter("@EmployeeId", employeeId);

                if (searchKey.IsNullOrEmpty())
                {
                    return await dbContext.Set<MinimumQuantityArticle>()
                        .FromSqlRaw($"EXEC SP_GET_MINIMUM_QUANTITY_ALARMS " +
                        $"@EmployeeId",
                        employee_Id).ToListAsync(ct);                    
                }
                else
                {
                    var search = new SqlParameter("@SEARCHKEY", searchKey);

                    return await dbContext.Set<MinimumQuantityArticle>()
                        .FromSqlRaw($"EXEC SP_GET_MINIMUM_QUANTITY_ALARMS " +
                        $"@EmployeeId, " +
                        $"@SEARCHKEY ",
                        employee_Id, search).ToListAsync(ct);                    
                }
            }, ct);
        }
        public async Task<IEnumerable<PurchaseOrderNotification>> GetNotificationsByModifiedPurchaseOrder(int modifiedPurchaseOrderId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.PurchaseOrderNotifications.AsNoTracking()
                                        .Include(i => i.ModifiedPurchaseOrder.ModificationReason)
                                        .Include(i => i.CustomerOrder.Customer)
                                        .Where(w => w.ModifiedPurchaseOrder.ModifiedPurchaseOrderId == modifiedPurchaseOrderId)
                                        .ToListAsync(ct);
            }, ct);
        }
        public async Task<IEnumerable<MinimumLocalWarehouseQuantityArticle>> GetMinimumLocalWarehouseQuantityAlarmsAsync(int employeeId, string? searchKey = null, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                var employee_Id = new SqlParameter("@EmployeeId", employeeId);

                if (searchKey.IsNullOrEmpty())
                    return await dbContext.Set<MinimumLocalWarehouseQuantityArticle>()
                        .FromSqlRaw($"EXEC SP_GET_MINIMUM_LOCAL_WAREHOUSE_QUANTITY_ALARMS " +
                        $"@EmployeeId",
                        employee_Id).ToListAsync(ct);
                else
                {
                    var search = new SqlParameter("@SEARCHKEY", searchKey);

                    return await dbContext.Set<MinimumLocalWarehouseQuantityArticle>()
                        .FromSqlRaw($"EXEC SP_GET_MINIMUM_LOCAL_WAREHOUSE_QUANTITY_ALARMS " +
                        $"@EmployeeId, " +
                        $"@SEARCHKEY ",
                        employee_Id, search).ToListAsync(ct);
                }
            }, ct);
        }

        public async Task<IEnumerable<LocalWarehouseAlarm>> GetLocalWarehouseAlarm(int employeeId, string? searchKey = null, CancellationToken ct = default)
        {
            var localWarehouseAlarmList = await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.LocalWarehouseAlarms.AsNoTracking()
                                        .Include(i=>i.DocumentType)
                                        .ToListAsync(ct);
            }, ct);

            throw new NotImplementedException();
        }
    }
}
