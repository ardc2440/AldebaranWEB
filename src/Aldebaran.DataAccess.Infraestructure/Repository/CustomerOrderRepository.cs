using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Infraestructure.Models;
using Aldebaran.Infraestructure.Common.Utils;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CustomerOrderRepository : RepositoryBase<AldebaranDbContext>, ICustomerOrderRepository
    {
        private readonly ISharedStringLocalizer _SharedLocalizer;
        public CustomerOrderRepository(IServiceProvider serviceProvider, ISharedStringLocalizer sharedLocalizer) : base(serviceProvider)
        {
            _SharedLocalizer = sharedLocalizer ?? throw new ArgumentNullException(nameof(ISharedStringLocalizer));
        }

        public async Task<CustomerOrder?> AddAsync(CustomerOrder customerOrder, CancellationToken ct = default)
        {
            return await ExecuteCommandAsync(async dbContext =>
            {
                var entity = new CustomerOrder
                {
                    CreationDate = customerOrder.CreationDate,
                    CustomerId = customerOrder.CustomerId,
                    CustomerNotes = customerOrder.CustomerNotes,
                    EmployeeId = customerOrder.EmployeeId,
                    OrderDate = customerOrder.OrderDate,
                    EstimatedDeliveryDate = customerOrder.EstimatedDeliveryDate,
                    StatusDocumentTypeId = customerOrder.StatusDocumentTypeId,
                    InternalNotes = customerOrder.InternalNotes,
                    OrderNumber = customerOrder.OrderNumber,
                    CustomerOrderDetails = new List<CustomerOrderDetail>()
                };

                foreach (var item in customerOrder.CustomerOrderDetails)
                {
                    entity.CustomerOrderDetails.Add(new CustomerOrderDetail
                    {
                        Brand = item.Brand,
                        CustomerOrderId = item.CustomerOrderId,
                        DeliveredQuantity = item.DeliveredQuantity,
                        ProcessedQuantity = item.ProcessedQuantity,
                        RequestedQuantity = item.RequestedQuantity,
                        ReferenceId = item.ReferenceId
                    });
                }
                try
                {
                    await dbContext.CustomerOrders.AddAsync(entity, ct);
                    await dbContext.SaveChangesAsync(ct);
                }
                catch (Exception)
                {
                    dbContext.Entry(entity).State = EntityState.Unchanged;
                    throw;
                }
                customerOrder.CustomerOrderId = entity.CustomerOrderId;
                customerOrder.OrderNumber = entity.OrderNumber;
                return customerOrder;
            }, ct);
        }

        public async Task<IEnumerable<CustomerOrder>> GetAsync(CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.CustomerOrders.AsNoTracking()
                            .Include(i => i.Customer.City.Department.Country)
                            .Include(i => i.Customer.IdentityType)
                            .Include(i => i.StatusDocumentType.DocumentType)
                            .Include(i => i.Employee.IdentityType) 
                            .Include(i => i.CustomerOrderDetails)
                            .OrderBy(o => o.OrderNumber)
                            .ToListAsync(ct);
            }, ct);
        }

        public async Task<IEnumerable<CustomerOrder>> GetAsync(string searchKey, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.CustomerOrders.AsNoTracking()
                            .Include(i => i.Customer.City.Department.Country)
                            .Include(i => i.Customer.IdentityType)
                            .Include(i => i.StatusDocumentType.DocumentType)
                            .Include(i => i.Employee.IdentityType)
                            .Include(i => i.CustomerOrderDetails)
                            .Where(i => i.InternalNotes.Contains(searchKey) ||
                                        i.CustomerNotes.Contains(searchKey) ||
                                        i.OrderNumber.Contains(searchKey) ||
                                        i.StatusDocumentType.StatusDocumentTypeName.Contains(searchKey) ||
                                        i.Employee.FullName.Contains(searchKey) ||
                                        i.Employee.DisplayName.Contains(searchKey) ||
                                        i.Employee.Area.AreaName.Contains(searchKey) ||
                                        i.Customer.City.Department.Country.CountryCode.Contains(searchKey) ||
                                        i.Customer.City.Department.Country.CountryName.Contains(searchKey) ||
                                        i.Customer.City.Department.DepartmentName.Contains(searchKey) ||
                                        i.Customer.City.CityName.Contains(searchKey) ||
                                        i.Customer.CustomerName.Contains(searchKey) ||
                                        i.Customer.CustomerAddress.Contains(searchKey) ||
                                        i.Customer.IdentityType.IdentityTypeCode.Contains(searchKey) ||
                                        i.Customer.IdentityType.IdentityTypeName.Contains(searchKey) ||
                                        i.Customer.IdentityNumber.Contains(searchKey) ||
                                        dbContext.Format(i.CreationDate, _SharedLocalizer["date:format"]).Contains(searchKey) ||
                                        dbContext.Format(i.OrderDate, _SharedLocalizer["date:format"]).Contains(searchKey) ||
                                        dbContext.Format(i.EstimatedDeliveryDate, _SharedLocalizer["date:format"]).Contains(searchKey)
                                        )
                            .OrderBy(o => o.OrderNumber)
                            .ToListAsync(ct);
            }, ct);
        }

        public async Task<CustomerOrder?> FindAsync(int customerOrderId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.CustomerOrders.AsNoTracking()
                            .Include(i => i.Customer.City.Department.Country)
                            .Include(i => i.Customer.IdentityType)
                            .Include(i => i.StatusDocumentType.DocumentType)
                            .Include(i => i.Employee.IdentityType)
                            .FirstOrDefaultAsync(i => i.CustomerOrderId.Equals(customerOrderId), ct);
            }, ct);
        }
        public async Task UpdateAsync(int customerOrderId, CustomerOrder customerOrder, Reason reason, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.CustomerOrders.Include(i => i.CustomerOrderDetails).FirstOrDefaultAsync(x => x.CustomerOrderId == customerOrderId, ct) ?? throw new KeyNotFoundException($"Pedido con id {customerOrderId} no existe.");
                entity.OrderDate = customerOrder.OrderDate;
                entity.EstimatedDeliveryDate = customerOrder.EstimatedDeliveryDate;
                entity.CustomerId = customerOrder.CustomerId;
                entity.InternalNotes = customerOrder.InternalNotes;
                entity.CustomerNotes = customerOrder.CustomerNotes;

                foreach (var item in entity.CustomerOrderDetails)
                    if (!customerOrder.CustomerOrderDetails.Any(i => i.CustomerOrderDetailId == item.CustomerOrderDetailId))
                        dbContext.CustomerOrderDetails.Remove(item);

                foreach (var item in customerOrder.CustomerOrderDetails)
                {
                    if (item.CustomerOrderDetailId.Equals(0))
                    {
                        entity.CustomerOrderDetails.Add(new CustomerOrderDetail()
                        {
                            Brand = item.Brand,
                            CustomerOrderId = item.CustomerOrderDetailId,
                            ReferenceId = item.ReferenceId,
                            RequestedQuantity = item.RequestedQuantity,
                            ProcessedQuantity = item.ProcessedQuantity,
                            DeliveredQuantity = item.DeliveredQuantity
                        });
                        continue;
                    }

                    var detail = entity.CustomerOrderDetails.FirstOrDefault(i => i.CustomerOrderDetailId == item.CustomerOrderDetailId);

                    if (detail != null)
                    {
                        detail.Brand = item.Brand;
                        detail.ReferenceId = item.ReferenceId;
                        detail.RequestedQuantity = item.RequestedQuantity;
                        detail.ProcessedQuantity = item.ProcessedQuantity;
                        detail.DeliveredQuantity = item.DeliveredQuantity;
                    }
                }

                var reasonEntity = new ModifiedCustomerOrder
                {
                    CustomerOrderId = customerOrderId,
                    ModificationReasonId = reason.ReasonId,
                    EmployeeId = reason.EmployeeId,
                    ModificationDate = reason.Date
                };
                try
                {
                    dbContext.ModifiedCustomerOrders.Add(reasonEntity);
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

        public async Task CancelAsync(int customerOrderId, short canceledStatusDocumentId, Reason reason, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.CustomerOrders.Include(i => i.CustomerOrderDetails).FirstOrDefaultAsync(x => x.CustomerOrderId == customerOrderId, ct) ?? throw new KeyNotFoundException($"Pedido con id {customerOrderId} no existe.");
                entity.StatusDocumentTypeId = canceledStatusDocumentId;
                var reasonEntity = new CanceledCustomerOrder
                {
                    CustomerOrderId = customerOrderId,
                    CancellationReasonId = reason.ReasonId,
                    EmployeeId = reason.EmployeeId,
                    CancellationDate = reason.Date
                };

                var alarms = await (from a in dbContext.Alarms
                                    join b in dbContext.AlarmMessages on a.AlarmMessageId equals b.AlarmMessageId
                                    join c in dbContext.AlarmTypes on b.AlarmTypeId equals c.AlarmTypeId
                                    join d in dbContext.DocumentTypes on c.DocumentTypeId equals d.DocumentTypeId
                                    where d.DocumentTypeCode.Equals("P") && a.DocumentId == customerOrderId && a.IsActive == true
                                    select (Alarm)a).ToListAsync();

                foreach (var alarm in alarms) alarm.IsActive = false;

                try
                {
                    dbContext.CanceledCustomerOrders.Add(reasonEntity);
                    await dbContext.SaveChangesAsync(ct);
                }
                catch (Exception)
                {
                    foreach (var alarm in alarms)
                    {
                        dbContext.Entry(alarm).State = EntityState.Unchanged;
                    }
                    dbContext.Entry(entity).State = EntityState.Unchanged;
                    dbContext.Entry(reasonEntity).State = EntityState.Unchanged;
                    throw;
                }
            }, ct);
        }

        public async Task CloseAsync(int customerOrderId, short ClosedStatusDocumentId, Reason reason, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.CustomerOrders.Include(i => i.CustomerOrderDetails).FirstOrDefaultAsync(x => x.CustomerOrderId == customerOrderId, ct) ?? throw new KeyNotFoundException($"Pedido con id {customerOrderId} no existe.");
                entity.StatusDocumentTypeId = ClosedStatusDocumentId;
                var reasonEntity = new ClosedCustomerOrder
                {
                    CustomerOrderId = customerOrderId,
                    CloseCustomerOrderReasonId = reason.ReasonId,
                    EmployeeId = reason.EmployeeId,
                    CloseDate = reason.Date
                };

                var alarms = await (from a in dbContext.Alarms
                                    join b in dbContext.AlarmMessages on a.AlarmMessageId equals b.AlarmMessageId
                                    join c in dbContext.AlarmTypes on b.AlarmTypeId equals c.AlarmTypeId
                                    join d in dbContext.DocumentTypes on c.DocumentTypeId equals d.DocumentTypeId
                                    where d.DocumentTypeCode.Equals("P") && a.DocumentId == customerOrderId && a.IsActive == true
                                    select (Alarm)a).ToListAsync();

                foreach (var alarm in alarms) alarm.IsActive = false;

                try
                {
                    dbContext.ClosedCustomerOrders.Add(reasonEntity);
                    await dbContext.SaveChangesAsync(ct);
                }
                catch (Exception)
                {
                    foreach (var alarm in alarms)
                    {
                        dbContext.Entry(alarm).State = EntityState.Unchanged;
                    }
                    dbContext.Entry(entity).State = EntityState.Unchanged;
                    dbContext.Entry(reasonEntity).State = EntityState.Unchanged;
                    throw;
                }
            }, ct);
        }
    }

}
