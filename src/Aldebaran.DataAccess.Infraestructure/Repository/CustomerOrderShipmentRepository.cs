using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Infraestructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CustomerOrderShipmentRepository : RepositoryBase<AldebaranDbContext>, ICustomerOrderShipmentRepository
    {
        private ICustomerOrderRepository _customerOrderRepository;
        public CustomerOrderShipmentRepository(IServiceProvider serviceProvider, ICustomerOrderRepository customerOrderRepository) : base(serviceProvider)
        {
            _customerOrderRepository = customerOrderRepository;
        }

        public async Task<CustomerOrderShipment> AddAsync(CustomerOrderShipment customerOrderShipment, CancellationToken ct)
        {
            return await ExecuteCommandAsync(async dbContext =>
            {
                var entity = new CustomerOrderShipment
                {
                    ShippingDate = customerOrderShipment.ShippingDate,
                    CreationDate = customerOrderShipment.CreationDate,
                    CustomerOrderId = customerOrderShipment.CustomerOrderId,
                    EmployeeId = customerOrderShipment.EmployeeId,
                    StatusDocumentTypeId = customerOrderShipment.StatusDocumentTypeId,
                    Notes = customerOrderShipment.Notes,
                    ShippingMethodId = customerOrderShipment.ShippingMethodId,
                    DeliveryNote = customerOrderShipment.DeliveryNote,
                    TrackingNumber = customerOrderShipment.TrackingNumber,
                    CustomerOrderShipmentDetails = new List<CustomerOrderShipmentDetail>()
                };

                foreach (var item in customerOrderShipment.CustomerOrderShipmentDetails)
                {
                    entity.CustomerOrderShipmentDetails.Add(new CustomerOrderShipmentDetail
                    {
                        DeliveredQuantity = item.DeliveredQuantity,
                        CustomerOrderDetailId = item.CustomerOrderDetailId
                    });
                }
                
                var orderEntity = await dbContext.CustomerOrders.FirstAsync(f => f.CustomerOrderId == entity.CustomerOrderId,ct);

                orderEntity.StatusDocumentTypeId = customerOrderShipment.CustomerOrder.StatusDocumentTypeId;

                var statusDocument = await dbContext.StatusDocumentTypes.AsNoTracking().FirstAsync(f => f.StatusDocumentTypeId == orderEntity.StatusDocumentTypeId);

                var alarms = new List<Alarm>();

                if (statusDocument.StatusOrder == 4)
                {
                    alarms = await (from a in dbContext.Alarms
                                    join b in dbContext.AlarmMessages on a.AlarmMessageId equals b.AlarmMessageId
                                    join c in dbContext.AlarmTypes on b.AlarmTypeId equals c.AlarmTypeId
                                    join d in dbContext.DocumentTypes on c.DocumentTypeId equals d.DocumentTypeId
                                    where d.DocumentTypeCode.Equals("P") && a.DocumentId == entity.CustomerOrderId && a.IsActive == true
                                    select (Alarm)a).ToListAsync();

                    foreach (var alarm in alarms) alarm.IsActive = false;
                }

                try
                {
                    await dbContext.CustomerOrderShipments.AddAsync(entity, ct);
                    await dbContext.SaveChangesAsync(ct);
                }
                catch (Exception)
                {
                    if (statusDocument.StatusOrder == 4)
                        foreach (var alarm in alarms)
                        {
                            dbContext.Entry(alarm).State = EntityState.Unchanged;
                        } 
                    dbContext.Entry(orderEntity).State = EntityState.Unchanged;
                    dbContext.Entry(entity).State = EntityState.Unchanged;
                    throw;
                }

                return entity;
            }, ct);
        }

        public async Task<IEnumerable<CustomerOrderShipment>> GetByCustomerOrderIdAsync(int customerOrderId, CancellationToken ct)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.CustomerOrderShipments.AsNoTracking()
               .Include(i => i.CustomerOrder)
               .Include(i => i.Employee)
               .Include(i => i.StatusDocumentType)
               .Include(i => i.ShippingMethod)
               .Include(i => i.CustomerOrderShipmentDetails)
               .Where(i => i.CustomerOrderId == customerOrderId)
               .ToListAsync(ct);
            }, ct);
        }

        public async Task UpdateAsync(int customerOrderShipmentId, CustomerOrderShipment customerOrderShipment, Reason reason, CancellationToken ct)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.CustomerOrderShipments.Include(i => i.CustomerOrderShipmentDetails).FirstOrDefaultAsync(x => x.CustomerOrderShipmentId == customerOrderShipmentId, ct) ?? throw new KeyNotFoundException($"Despacho de pedido con id {customerOrderShipmentId} no existe.");

                entity.ShippingDate = customerOrderShipment.ShippingDate;
                entity.CreationDate = customerOrderShipment.CreationDate;
                entity.CustomerOrderId = customerOrderShipment.CustomerOrderId;
                entity.EmployeeId = customerOrderShipment.EmployeeId;
                entity.StatusDocumentTypeId = customerOrderShipment.StatusDocumentTypeId;
                entity.Notes = customerOrderShipment.Notes;
                entity.ShippingMethodId = customerOrderShipment.ShippingMethodId;
                entity.DeliveryNote = customerOrderShipment.DeliveryNote;
                entity.TrackingNumber = customerOrderShipment.TrackingNumber;

                var oldDetailSum = entity.CustomerOrderShipmentDetails.Sum(s => s.DeliveredQuantity);

                foreach (var item in customerOrderShipment.CustomerOrderShipmentDetails)
                {
                    if (item.CustomerOrderShipmentDetailId > 0)
                    {
                        var detail = entity.CustomerOrderShipmentDetails.FirstOrDefault(i => i.CustomerOrderShipmentDetailId == item.CustomerOrderShipmentDetailId);
                        if (detail != null)
                        {
                            detail.DeliveredQuantity = item.DeliveredQuantity;
                            detail.CustomerOrderDetailId = item.CustomerOrderDetailId;
                        }
                        continue;
                    }

                    entity.CustomerOrderShipmentDetails.Add(new CustomerOrderShipmentDetail()
                    {
                        CustomerOrderDetailId = item.CustomerOrderDetailId,
                        DeliveredQuantity = item.DeliveredQuantity
                    });
                }

                foreach (var item in entity.CustomerOrderShipmentDetails)
                {
                    if (!customerOrderShipment.CustomerOrderShipmentDetails.Any(i => i.CustomerOrderShipmentId == item.CustomerOrderShipmentId))
                        dbContext.CustomerOrderShipmentDetails.Remove(item);
                }

                var reasonEntity = new ModifiedOrderShipment
                {
                    CustomerOrderShipmentId = customerOrderShipmentId,
                    ModificationReasonId = reason.ReasonId,
                    EmployeeId = reason.EmployeeId,
                    ModificationDate = reason.Date
                };
                                
                var orderEntity = dbContext.CustomerOrders.First(w => w.CustomerOrderId == entity.CustomerOrderId);

                orderEntity.StatusDocumentTypeId = customerOrderShipment.CustomerOrder.StatusDocumentTypeId;

                var statusDocument = await dbContext.StatusDocumentTypes.AsNoTracking().FirstAsync(f => f.StatusDocumentTypeId == orderEntity.StatusDocumentTypeId);

                var alarms = new List<Alarm>();

                if (statusDocument.StatusOrder == 4)
                {
                    alarms = await (from a in dbContext.Alarms
                                    join b in dbContext.AlarmMessages on a.AlarmMessageId equals b.AlarmMessageId
                                    join c in dbContext.AlarmTypes on b.AlarmTypeId equals c.AlarmTypeId
                                    join d in dbContext.DocumentTypes on c.DocumentTypeId equals d.DocumentTypeId
                                    where d.DocumentTypeCode.Equals("P") && a.DocumentId == entity.CustomerOrderId && a.IsActive == true
                                    select (Alarm)a).ToListAsync();

                    foreach (var alarm in alarms) alarm.IsActive = false;
                }

                try
                {
                    dbContext.ModifiedOrderShipments.Add(reasonEntity);
                    await dbContext.SaveChangesAsync(ct);
                }
                catch (Exception)
                {
                    if (statusDocument.StatusOrder == 4)
                        foreach (var alarm in alarms)
                        {
                            dbContext.Entry(alarm).State = EntityState.Unchanged;
                        }
                    dbContext.Entry(orderEntity).State = EntityState.Unchanged;
                    dbContext.Entry(entity).State = EntityState.Unchanged;
                    dbContext.Entry(reasonEntity).State = EntityState.Unchanged;
                    throw;
                }
            }, ct);
        }

        public async Task<CustomerOrderShipment?> FindAsync(int customerOrderShipmentId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.CustomerOrderShipments.AsNoTracking()
                            .Include(i => i.CustomerOrder)
                            .Include(i => i.Employee)
                            .Include(i => i.ShippingMethod)
                            .Include(i => i.StatusDocumentType)
                            .FirstOrDefaultAsync(i => i.CustomerOrderShipmentId == customerOrderShipmentId, ct);
            }, ct);
        }

        public async Task CancelAsync(int customerOrderShipmentId, short canceledStatusDocumentId, Reason reason, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.CustomerOrderShipments.Include(i => i.CustomerOrderShipmentDetails).FirstOrDefaultAsync(x => x.CustomerOrderShipmentId == customerOrderShipmentId, ct) ?? throw new KeyNotFoundException($"Traslado con id {customerOrderShipmentId} no existe.");
                entity.StatusDocumentTypeId = canceledStatusDocumentId;

                var reasonEntity = new CanceledOrderShipment
                {
                    CustomerOrderShipmentId = customerOrderShipmentId,
                    CancellationReasonId = reason.ReasonId,
                    EmployeeId = reason.EmployeeId,
                    CancellationDate = reason.Date
                };
                                                
                try
                {
                    dbContext.CanceledOrderShipments.Add(reasonEntity);
                    await dbContext.SaveChangesAsync(ct);
                }
                catch (Exception)
                {
                    dbContext.Entry(entity).State = EntityState.Unchanged;
                    dbContext.Entry(reasonEntity).State = EntityState.Unchanged;
                    throw;
                }
            }, ct);
        }
    }
}
