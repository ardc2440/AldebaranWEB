using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Enums;
using Aldebaran.DataAccess.Infraestructure.Models;
using Aldebaran.Infraestructure.Common.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class PurchaseOrderRepository : RepositoryBase<AldebaranDbContext>, IPurchaseOrderRepository
    {
        private readonly ISharedStringLocalizer _SharedLocalizer;
        public PurchaseOrderRepository(IServiceProvider serviceProvider, ISharedStringLocalizer sharedLocalizer) : base(serviceProvider)
        {
            _SharedLocalizer = sharedLocalizer ?? throw new ArgumentNullException(nameof(ISharedStringLocalizer));
        }

        public async Task<PurchaseOrder> AddAsync(PurchaseOrder item, CancellationToken ct = default)
        {
            return await ExecuteCommandAsync(async dbContext =>
            {
                await dbContext.PurchaseOrders.AddAsync(item, ct);
                await dbContext.SaveChangesAsync(ct);
                return item;
            }, ct);
        }

        public async Task CancelAsync(int purchaseOrderId, Reason reason, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.PurchaseOrders.FirstOrDefaultAsync(x => x.PurchaseOrderId == purchaseOrderId, ct) ?? throw new KeyNotFoundException($"Orden con id {purchaseOrderId} no existe.");
                var documentType = await dbContext.DocumentTypes.AsNoTracking().FirstAsync(f => f.DocumentTypeCode == "O", ct);
                var statutsDocumentType = await dbContext.StatusDocumentTypes.AsNoTracking().FirstAsync(f => f.DocumentTypeId == documentType.DocumentTypeId && f.StatusOrder == 3, ct);
                entity.StatusDocumentTypeId = statutsDocumentType.StatusDocumentTypeId;

                var alarms = await (from a in dbContext.Alarms
                                    join b in dbContext.AlarmMessages on a.AlarmMessageId equals b.AlarmMessageId
                                    join c in dbContext.AlarmTypes on b.AlarmTypeId equals c.AlarmTypeId
                                    join d in dbContext.DocumentTypes on c.DocumentTypeId equals d.DocumentTypeId
                                    where d.DocumentTypeCode.Equals("O") && a.DocumentId == purchaseOrderId && a.IsActive == true
                                    select (Alarm)a).ToListAsync();

                foreach (var alarm in alarms) alarm.IsActive = false;

                var reasonEntity = new CanceledPurchaseOrder
                {
                    PurchaseOrderId = purchaseOrderId,
                    CancellationReasonId = reason.ReasonId,
                    EmployeeId = reason.EmployeeId,
                    CancellationDate = reason.Date
                };
                try
                {
                    dbContext.CanceledPurchaseOrders.Add(reasonEntity);
                    await dbContext.SaveChangesAsync(ct);
                }
                catch
                {
                    foreach (var alarm in alarms)
                    {
                        dbContext.Entry(alarm).State = EntityState.Unchanged;
                    };
                    dbContext.Entry(reasonEntity).State = EntityState.Unchanged;
                    dbContext.Entry(entity).State = EntityState.Unchanged;
                    throw;
                }
            }, ct);
        }

        public async Task ConfirmAsync(int purchaseOrderId, PurchaseOrder purchaseOrder, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.PurchaseOrders.FirstOrDefaultAsync(x => x.PurchaseOrderId == purchaseOrderId, ct) ?? throw new KeyNotFoundException($"Orden con id {purchaseOrderId} no existe.");
                entity.RealReceiptDate = purchaseOrder.RealReceiptDate;
                entity.ImportNumber = purchaseOrder.ImportNumber;
                entity.EmbarkationPort = purchaseOrder.EmbarkationPort;
                entity.ProformaNumber = purchaseOrder.ProformaNumber;

                var documentType = await dbContext.DocumentTypes.AsNoTracking().FirstAsync(f => f.DocumentTypeCode == "O", ct);
                var statutsDocumentType = await dbContext.StatusDocumentTypes.AsNoTracking().FirstAsync(f => f.DocumentTypeId == documentType.DocumentTypeId && f.StatusOrder == 2, ct);
                entity.StatusDocumentTypeId = statutsDocumentType.StatusDocumentTypeId;

                // Details
                var details = await dbContext.PurchaseOrderDetails.Where(x => x.PurchaseOrderId == purchaseOrderId).ToListAsync(ct);
                foreach (var detail in purchaseOrder.PurchaseOrderDetails)
                {
                    var detailToUpdate = details.FirstOrDefault(f => f.PurchaseOrderDetailId == detail.PurchaseOrderDetailId);
                    if (detailToUpdate != null)
                    {
                        detailToUpdate.ReceivedQuantity = detail.ReceivedQuantity;

                        if (detailToUpdate.WarehouseId != detail.WarehouseId)
                            detailToUpdate.WarehouseId = detail.WarehouseId;
                    }
                }
                // Alarms
                var alarms = await (from a in dbContext.Alarms
                                    join b in dbContext.AlarmMessages on a.AlarmMessageId equals b.AlarmMessageId
                                    join c in dbContext.AlarmTypes on b.AlarmTypeId equals c.AlarmTypeId
                                    join d in dbContext.DocumentTypes on c.DocumentTypeId equals d.DocumentTypeId
                                    where d.DocumentTypeCode.Equals("O") && a.DocumentId == entity.PurchaseOrderId && a.IsActive == true
                                    select (Alarm)a).ToListAsync();

                foreach (var alarm in alarms) alarm.IsActive = false;

                try
                {
                    await dbContext.SaveChangesAsync(ct);
                }
                catch
                {
                    foreach (var alarm in alarms)
                    {
                        dbContext.Entry(alarm).State = EntityState.Unchanged;
                    }
                    dbContext.Entry(details).State = EntityState.Unchanged;
                    dbContext.Entry(entity).State = EntityState.Unchanged;
                    throw;
                }
            }, ct);
        }

        public async Task<PurchaseOrder?> FindAsync(int purchaseOrderId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.PurchaseOrders.AsNoTracking()
                      .Include(i => i.Employee.Area)
                      .Include(i => i.Employee.IdentityType)
                      .Include(i => i.ForwarderAgent.Forwarder)
                      .Include(i => i.Provider.IdentityType)
                      .Include(i => i.ShipmentForwarderAgentMethod.ShipmentMethod)
                      .Include(i => i.ShipmentForwarderAgentMethod.ForwarderAgent)
                      .Include(i => i.StatusDocumentType.DocumentType)                      
                      .Where(w => w.PurchaseOrderId == purchaseOrderId)
                      .FirstOrDefaultAsync(ct);
            }, ct);
        }

        public async Task<(IEnumerable<PurchaseOrder>,int)> GetAsync(int skip, int top, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                var a = dbContext.PurchaseOrders.AsNoTracking()
                        .Include(i => i.Employee.Area)
                        .Include(i => i.Employee.IdentityType)
                        .Include(i => i.ForwarderAgent.Forwarder)
                        .Include(i => i.Provider.IdentityType)
                        .Include(i => i.ShipmentForwarderAgentMethod.ShipmentMethod)
                        .Include(i => i.ShipmentForwarderAgentMethod.ForwarderAgent)
                        .Include(i => i.StatusDocumentType.DocumentType)
                        .OrderByDescending(o => o.OrderNumber);

                return (await a.Skip(skip).Take(top).ToListAsync(ct), await a.CountAsync(ct));
            }, ct);
        }

        public async Task<(IEnumerable<PurchaseOrder>,int)> GetAsync(int skip, int top, string searchKey, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                var a = dbContext.PurchaseOrders.AsNoTracking()
                           .Include(i => i.Employee.Area)
                           .Include(i => i.Employee.IdentityType)
                           .Include(i => i.ForwarderAgent.Forwarder)
                           .Include(i => i.Provider.IdentityType)
                           .Include(i => i.ShipmentForwarderAgentMethod.ShipmentMethod)
                           .Include(i => i.ShipmentForwarderAgentMethod.ForwarderAgent)
                           .Include(i => i.StatusDocumentType.DocumentType)
                           .Where(w => w.OrderNumber.Contains(searchKey) ||
                                       w.ImportNumber.Contains(searchKey) ||
                                       w.Provider.ProviderName.Contains(searchKey) ||
                                       w.ForwarderAgent.Forwarder.ForwarderName.Contains(searchKey) ||
                                       w.ForwarderAgent.ForwarderAgentName.Contains(searchKey) ||
                                       w.EmbarkationPort.Contains(searchKey) ||
                                       w.ProformaNumber.Contains(searchKey) ||
                                       dbContext.Format(w.CreationDate, _SharedLocalizer["date:format"]).Contains(searchKey) ||
                                       dbContext.Format(w.ExpectedReceiptDate, _SharedLocalizer["date:format"]).Contains(searchKey) ||
                                       dbContext.Format(w.RequestDate, _SharedLocalizer["date:format"]).Contains(searchKey) ||
                                       (w.RealReceiptDate.HasValue && dbContext.Format(w.RealReceiptDate.Value, _SharedLocalizer["date:format"]).Contains(searchKey)))
                           .OrderByDescending(o => o.OrderNumber);

                return (await a.Skip(skip).Take(top).ToListAsync(), await a.CountAsync(ct));
            }, ct);
        }

        public async Task<IEnumerable<PurchaseOrder>> GetTransitByReferenceIdAsync(int referenceId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.PurchaseOrders.AsNoTracking()
                           .Include(i => i.PurchaseOrderDetails)
                           .Include(i => i.PurchaseOrderActivities)
                           .Where(w => w.StatusDocumentType.StatusOrder == 1 && dbContext.PurchaseOrderDetails.AsNoTracking().Any(d => d.PurchaseOrderId == w.PurchaseOrderId && d.ReferenceId == referenceId))
                           .OrderBy(o => o.OrderNumber)
                           .ToListAsync(ct);
            }, ct);
        }

        public async Task<int> UpdateAsync(int purchaseOrderId, PurchaseOrder purchaseOrder, Reason reason, IEnumerable<CustomerOrderAffectedByPurchaseOrderUpdate> ordersAffected, CancellationToken ct = default)
        {
            return await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.PurchaseOrders
                            .FirstOrDefaultAsync(x => x.PurchaseOrderId == purchaseOrderId, ct) ?? throw new KeyNotFoundException($"Orden con id {purchaseOrderId} no existe.");

                var oldExpectedReceiptDate = entity.ExpectedReceiptDate;

                entity.RequestDate = purchaseOrder.RequestDate;
                entity.ExpectedReceiptDate = purchaseOrder.ExpectedReceiptDate;
                entity.ProviderId = purchaseOrder.ProviderId;
                entity.ForwarderAgentId = purchaseOrder.ForwarderAgentId;
                entity.ShipmentForwarderAgentMethodId = purchaseOrder.ShipmentForwarderAgentMethodId;
                entity.ProformaNumber = purchaseOrder.ProformaNumber;

                // Details
                var details = await dbContext.PurchaseOrderDetails.Where(x => x.PurchaseOrderId == purchaseOrderId).ToListAsync(ct);
                dbContext.PurchaseOrderDetails.RemoveRange(details);
                entity.PurchaseOrderDetails = purchaseOrder.PurchaseOrderDetails;
                IEnumerable<PurchaseOrderNotification> purchaseOrderNotifications = new List<PurchaseOrderNotification>();

                List<PurchaseOrderTransitAlarm> purchaseOrderTransitAlarm = new List<PurchaseOrderTransitAlarm>();

                if (ordersAffected.Any())
                {
                    purchaseOrderNotifications = ordersAffected.Select(s => new PurchaseOrderNotification
                    {
                        CustomerOrderId = s.CustomerOrderId,
                        NotificationId = string.Empty,
                        NotificationState = NotificationStatus.Pending,
                        NotifiedMailList = (dbContext.CustomerOrders.AsNoTracking()
                                    .Include(i => i.Customer)
                                    .FirstOrDefault(f => f.CustomerOrderId == s.CustomerOrderId)).Customer.Email
                    });

                    purchaseOrderTransitAlarm.Add(new PurchaseOrderTransitAlarm { OldExpectedReceiptDate = oldExpectedReceiptDate });
                }

                var reasonEntity = new ModifiedPurchaseOrder
                {
                    PurchaseOrderId = purchaseOrderId,
                    ModificationReasonId = reason.ReasonId,
                    EmployeeId = reason.EmployeeId,
                    ModificationDate = reason.Date
                };

                if (purchaseOrderNotifications.Any())
                {
                    reasonEntity.PurchaseOrderNotifications = purchaseOrderNotifications.ToList();
                    reasonEntity.PurchaseOrderTransitAlarms = purchaseOrderTransitAlarm;
                }

                try
                {
                    dbContext.ModifiedPurchaseOrders.Add(reasonEntity);
                    await dbContext.SaveChangesAsync(ct);
                    return reasonEntity.ModifiedPurchaseOrderId;
                }
                catch
                {
                    dbContext.Entry(entity).State = EntityState.Unchanged;
                    dbContext.Entry(reasonEntity).State = EntityState.Unchanged;
                    throw;
                }
            }, ct);
        }

        public async Task<IEnumerable<CustomerOrderAffectedByPurchaseOrderUpdate>> GetAffectedCustomerOrders(int purchaseOrderId, DateTime newExpectedReceiptDate, IEnumerable<PurchaseOrderDetail> purchaseOrderDetails, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                var purchaseOrderIdParameter = new SqlParameter("@PURCHASEORDERID", purchaseOrderId);
                var newExpectedReceiptDateParameter = new SqlParameter("@NEWEXPECTEDRECIPDATE", newExpectedReceiptDate);
                var purchaseOrderDetailsParameter = new SqlParameter("@PURCHASEORDERDETAILQUANTITIES", string.Join(";", purchaseOrderDetails.Select(s => $"{s.ReferenceId}-{s.RequestedQuantity}")));

                return await dbContext.Set<CustomerOrderAffectedByPurchaseOrderUpdate>()
                    .FromSqlRaw($"EXEC SP_CUSTOMER_ORDERS_AFFECTED_BY_PURCHASE_ORDER_UPDATE " +
                    $"@PURCHASEORDERID, @NEWEXPECTEDRECIPDATE, @PURCHASEORDERDETAILQUANTITIES",
                    purchaseOrderIdParameter, newExpectedReceiptDateParameter, purchaseOrderDetailsParameter).ToListAsync(ct);
            }, ct);
        }

        public async Task<IEnumerable<CustomerOrderAffectedByPurchaseOrderUpdate>> GetAffectedCustomerOrders(int purchaseOrderId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                var purchaseOrderIdParameter = new SqlParameter("@PURCHASEORDERID", purchaseOrderId);

                return await dbContext.Set<CustomerOrderAffectedByPurchaseOrderUpdate>()
                    .FromSqlRaw($"EXEC SP_CUSTOMER_ORDERS_POSSIBLY_AFFECTED_BY_PURCHASE_ORDER_ID " +
                    $"@PURCHASEORDERID",
                    purchaseOrderIdParameter).ToListAsync(ct);
            }, ct);
        }

        public async Task<(IEnumerable<PurchaseOrder> purchaseOrders, int count)> GetAsync(int skip, int take, string filter, string orderBy, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                var query = dbContext.PurchaseOrders.AsNoTracking()
                    .Include(i => i.Employee.Area)
                    .Include(i => i.Employee.IdentityType)
                    .Include(i => i.ForwarderAgent.Forwarder)
                    .Include(i => i.Provider.IdentityType)
                    .Include(i => i.ShipmentForwarderAgentMethod.ShipmentMethod)
                    .Include(i => i.ShipmentForwarderAgentMethod.ForwarderAgent)
                    .Include(i => i.StatusDocumentType.DocumentType)
                    .AsQueryable();
                if (!string.IsNullOrEmpty(filter))
                {
                    query = query.Where(filter);
                }
                if (!string.IsNullOrEmpty(orderBy))
                {
                    query = query.OrderBy(orderBy);
                }
                var count = query.Count();
                var data = await query.Skip(skip).Take(take).ToListAsync(ct);
                return (data, count);
            }, ct);
        }
    }
}
