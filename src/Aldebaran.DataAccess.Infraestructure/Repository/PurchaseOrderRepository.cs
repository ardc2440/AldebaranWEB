using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Enums;
using Aldebaran.DataAccess.Infraestructure.Models;
using Aldebaran.Infraestructure.Common.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
                    }
                    ;
                    dbContext.Entry(reasonEntity).State = EntityState.Unchanged;
                    dbContext.Entry(entity).State = EntityState.Unchanged;
                    throw;
                }
            }, ct);
        }

        public async Task ConfirmAsync(int purchaseOrderId, PurchaseOrder purchaseOrder, int? approvalEmployeeId, string? approvalReason, CancellationToken ct = default)
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

                // Obtener referencias con saldo <= 0 antes de guardar
                var referenceIds = details.Select(d => d.ReferenceId).Distinct().ToList();

                var referencesWithZeroOrLessStock = await dbContext.Set<ReferencesWarehouse>()
                    .Where(rw => referenceIds.Contains(rw.ReferenceId))
                    .Where(rw => rw.Quantity <= 0)
                    .Select(rw => rw.ReferenceId)
                    .ToListAsync(ct);

                var referenceIdList = string.Join(",", referencesWithZeroOrLessStock);

                try
                {
                    if (!string.IsNullOrEmpty(approvalReason))
                    {
                        var adjustmentLog = CreateAdjustmentLog(purchaseOrderId, purchaseOrder.StatusDocumentTypeId, approvalEmployeeId ?? 0, approvalReason);

                        dbContext.PurchaseOrderAdjustmentLogs.Add(adjustmentLog);
                    }

                    await dbContext.SaveChangesAsync(ct);

                    if (referencesWithZeroOrLessStock.Any())
                        await dbContext.Database.ExecuteSqlInterpolatedAsync($"EXEC dbo.SP_AUTOMATIC_CUSTOMER_ORDER_IN_PROCESS_GENERATION @DocumentType = 'O', @DocumentId = {purchaseOrderId}, @ReferenceIdList = {referenceIdList}", ct);
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

        public async Task<(IEnumerable<PurchaseOrder>, int)> GetAsync(int skip, int top, CancellationToken ct = default)
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

        public async Task<(IEnumerable<PurchaseOrder>, int)> GetAsync(int skip, int top, string searchKey, CancellationToken ct = default)
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
                    .Include(x => x.PurchaseOrderDetails)
                    .FirstOrDefaultAsync(x => x.PurchaseOrderId == purchaseOrderId, ct) ?? throw new KeyNotFoundException($"Orden con id {purchaseOrderId} no existe.");

                var oldExpectedReceiptDate = entity.ExpectedReceiptDate;

                UpdateHeader(entity, purchaseOrder);

                SynchronizeDetails(dbContext, entity, purchaseOrder, purchaseOrderId);

                var reasonEntity = CreateModificationReason(dbContext, purchaseOrderId, reason, ordersAffected, oldExpectedReceiptDate);

                try
                {
                    if (reasonEntity is not null)
                        dbContext.ModifiedPurchaseOrders.Add(reasonEntity);

                    await dbContext.SaveChangesAsync(ct);

                    return reasonEntity?.ModifiedPurchaseOrderId ?? 0;
                }
                catch
                {
                    dbContext.Entry(reasonEntity).State = EntityState.Detached;

                    throw;
                }
            }, ct);
        }

        private static void UpdateHeader(PurchaseOrder entity, PurchaseOrder purchaseOrder)
        {
            entity.RequestDate = purchaseOrder.RequestDate;
            entity.ExpectedReceiptDate = purchaseOrder.ExpectedReceiptDate;
            entity.ProviderId = purchaseOrder.ProviderId;
            entity.ForwarderAgentId = purchaseOrder.ForwarderAgentId;
            entity.ShipmentForwarderAgentMethodId = purchaseOrder.ShipmentForwarderAgentMethodId;
            entity.ProformaNumber = purchaseOrder.ProformaNumber;

            if (entity.StatusDocumentTypeId != purchaseOrder.StatusDocumentTypeId)
            {
                entity.StatusDocumentTypeId = purchaseOrder.StatusDocumentTypeId;                
                entity.RealReceiptDate = purchaseOrder.RealReceiptDate;
                entity.ImportNumber = purchaseOrder.ImportNumber;
                entity.EmbarkationPort = purchaseOrder.EmbarkationPort;
                entity.ProformaNumber = purchaseOrder.ProformaNumber;
            }
        }

        private static void SynchronizeDetails(DbContext dbContext, PurchaseOrder entity, PurchaseOrder purchaseOrder, int purchaseOrderId)
        {
            var currentDetails = entity.PurchaseOrderDetails.ToList();

            var detailsToDelete = currentDetails
                                    .Where(current => !purchaseOrder.PurchaseOrderDetails.Any(incoming => incoming.PurchaseOrderDetailId == current.PurchaseOrderDetailId))
                                    .ToList();

            dbContext.Set<PurchaseOrderDetail>().RemoveRange(detailsToDelete);

            foreach (var currentDetail in currentDetails)
            {
                var incomingDetail = purchaseOrder.PurchaseOrderDetails
                                        .FirstOrDefault(x => x.PurchaseOrderDetailId == currentDetail.PurchaseOrderDetailId);

                if (incomingDetail is null)
                    continue;

                currentDetail.RequestedQuantity = incomingDetail.RequestedQuantity;
                currentDetail.ReceivedQuantity = incomingDetail.ReceivedQuantity;
                currentDetail.WarehouseId = incomingDetail.WarehouseId;
            }

            var detailsToInsert = purchaseOrder.PurchaseOrderDetails
                                    .Where(incoming => incoming.PurchaseOrderDetailId <= 0 || !currentDetails.Any(current => current.PurchaseOrderDetailId == incoming.PurchaseOrderDetailId))
                                    .ToList();

            foreach (var detail in detailsToInsert)
            {
                detail.PurchaseOrderId = purchaseOrderId;
                entity.PurchaseOrderDetails.Add(detail);
            }
        }

        public async Task<int> DenyAdjustmentApproval(PurchaseOrderAdjustmentLog denyAdjustmentApproval, CancellationToken ct = default)
        {
            return await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.PurchaseOrders.FirstOrDefaultAsync(x => x.PurchaseOrderId == denyAdjustmentApproval.PurchaseOrderId, ct) ?? throw new KeyNotFoundException($"Orden con id {denyAdjustmentApproval.PurchaseOrderId} no existe.");
                
                entity.StatusDocumentTypeId = denyAdjustmentApproval.NewStatusDocumentTypeId;

                var adjustmentLog = CreateAdjustmentLog(denyAdjustmentApproval.PurchaseOrderId, denyAdjustmentApproval.NewStatusDocumentTypeId, denyAdjustmentApproval.EmployeeId, denyAdjustmentApproval.Reason);
                
                dbContext.PurchaseOrderAdjustmentLogs.Add(adjustmentLog);

                await dbContext.SaveChangesAsync(ct);
                
                return adjustmentLog.PurchaseOrderAdjustmentLogId;
            }, ct);
        }

        private static ModifiedPurchaseOrder CreateModificationReason(AldebaranDbContext dbContext, int purchaseOrderId, Reason reason, IEnumerable<CustomerOrderAffectedByPurchaseOrderUpdate> ordersAffected, DateTime oldExpectedReceiptDate)
        {
            var result = new ModifiedPurchaseOrder
            {
                PurchaseOrderId = purchaseOrderId,
                ModificationReasonId = reason.ReasonId,
                EmployeeId = reason.EmployeeId,
                ModificationDate = reason.Date
            };

            AttachNotifications(dbContext, result, ordersAffected, oldExpectedReceiptDate);

            return result;
        }

        private static void AttachNotifications(AldebaranDbContext dbContext, ModifiedPurchaseOrder reasonEntity, IEnumerable<CustomerOrderAffectedByPurchaseOrderUpdate> ordersAffected, DateTime oldExpectedReceiptDate)
        {
            if (!ordersAffected.Any())
                return;

            var notifications = ordersAffected.Select(s =>
                    new PurchaseOrderNotification
                    {
                        CustomerOrderId = s.CustomerOrderId,
                        NotificationId = string.Empty,
                        NotificationState = NotificationStatus.Pending,
                        NotifiedMailList = dbContext.CustomerOrders
                                            .AsNoTracking()
                                            .Include(i => i.Customer)
                                            .First(f => f.CustomerOrderId == s.CustomerOrderId)
                                            .Customer.Email
                    }).ToList();

            reasonEntity.PurchaseOrderNotifications = notifications;

            reasonEntity.PurchaseOrderTransitAlarms = new List<PurchaseOrderTransitAlarm>
                                                        {
                                                            new PurchaseOrderTransitAlarm
                                                                {
                                                                    OldExpectedReceiptDate = oldExpectedReceiptDate
                                                                }
                                                        };
        }

        private static PurchaseOrderAdjustmentLog CreateAdjustmentLog(int purchaseOrderId, short statusId, int employeeId, string reason)
        {
            var result = new PurchaseOrderAdjustmentLog
            {
                PurchaseOrderId = purchaseOrderId,
                NewStatusDocumentTypeId = statusId,
                EmployeeId = employeeId,
                Reason = reason,
                CreatedDate = DateTime.Now
            };

            return result;
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

        /* Logs */
        public async Task<(IEnumerable<ModifiedPurchaseOrder>, int count)> GetPurchaseOrderModificationsLogAsync(int skip, int top, string searchKey, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                var a = dbContext.ModifiedPurchaseOrders.AsNoTracking()
                            .Include(i => i.PurchaseOrder.Provider)
                            .Include(i => i.PurchaseOrder.Employee)
                            .Include(i => i.Employee)
                            .Include(i => i.ModificationReason)
                            .Where(i => (i.PurchaseOrder.OrderNumber.Contains(searchKey) ||
                                         i.PurchaseOrder.Provider.ProviderName.Contains(searchKey) ||
                                         i.PurchaseOrder.Provider.IdentityNumber.Contains(searchKey) ||
                                         i.Employee.FullName.Contains(searchKey) ||
                                         i.ModificationReason.ModificationReasonName.Contains(searchKey) ||
                                         dbContext.Format(i.ModificationDate, _SharedLocalizer["date:format"]).Contains(searchKey) ||
                                         dbContext.Format(i.PurchaseOrder.RequestDate, _SharedLocalizer["date:format"]).Contains(searchKey) ||
                                         dbContext.Format(i.PurchaseOrder.ExpectedReceiptDate, _SharedLocalizer["date:format"]).Contains(searchKey))
                                         || searchKey.IsNullOrEmpty())
                            .OrderByDescending(o => o.PurchaseOrder.OrderNumber);

                return (await a.Skip(skip).Take(top).ToListAsync(), await a.CountAsync(ct));
            }, ct);
        }

        public async Task<(IEnumerable<CanceledPurchaseOrder>, int count)> GetPurchaseOrderCancellationsLogAsync(int skip, int top, string searchKey, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                var a = dbContext.CanceledPurchaseOrders.AsNoTracking()
                            .Include(i => i.PurchaseOrder.Provider)
                            .Include(i => i.PurchaseOrder.Employee)
                            .Include(i => i.Employee)
                            .Include(i => i.CancellationReason)
                            .Where(i => (i.PurchaseOrder.OrderNumber.Contains(searchKey) ||
                                         i.PurchaseOrder.Provider.ProviderName.Contains(searchKey) ||
                                         i.PurchaseOrder.Provider.IdentityNumber.Contains(searchKey) ||
                                         i.Employee.FullName.Contains(searchKey) ||
                                         i.CancellationReason.CancellationReasonName.Contains(searchKey) ||
                                         dbContext.Format(i.CancellationDate, _SharedLocalizer["date:format"]).Contains(searchKey) ||
                                         dbContext.Format(i.PurchaseOrder.RequestDate, _SharedLocalizer["date:format"]).Contains(searchKey) ||
                                         dbContext.Format(i.PurchaseOrder.ExpectedReceiptDate, _SharedLocalizer["date:format"]).Contains(searchKey))
                                         || searchKey.IsNullOrEmpty())
                            .OrderByDescending(o => o.PurchaseOrder.OrderNumber);

                return (await a.Skip(skip).Take(top).ToListAsync(), await a.CountAsync(ct));
            }, ct);
        }
    }
}
