using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Infraestructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CustomerOrdersInProcessRepository : RepositoryBase<AldebaranDbContext>, ICustomerOrdersInProcessRepository
    {
        public CustomerOrdersInProcessRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<CustomerOrdersInProcess> AddAsync(CustomerOrdersInProcess customerOrdersInProcess, CancellationToken ct)
        {
            return await ExecuteCommandAsync(async dbContext =>
            {
                var entity = new CustomerOrdersInProcess
                {
                    CustomerOrderId = customerOrdersInProcess.CustomerOrderId,
                    EmployeeRecipientId = customerOrdersInProcess.EmployeeRecipientId,
                    EmployeeId = customerOrdersInProcess.EmployeeId,
                    Notes = customerOrdersInProcess.Notes,
                    ProcessDate = customerOrdersInProcess.ProcessDate,
                    CreationDate = customerOrdersInProcess.CreationDate,
                    ProcessSatelliteId = customerOrdersInProcess.ProcessSatelliteId,
                    StatusDocumentTypeId = customerOrdersInProcess.StatusDocumentTypeId,
                    TransferDatetime = customerOrdersInProcess.TransferDatetime,
                    CustomerOrderInProcessDetails = new List<CustomerOrderInProcessDetail>()
                };

                foreach (var item in customerOrdersInProcess.CustomerOrderInProcessDetails)
                {
                    entity.CustomerOrderInProcessDetails.Add(new CustomerOrderInProcessDetail
                    {
                        Brand = item.Brand,
                        CustomerOrderInProcessId = item.CustomerOrderInProcessId,
                        ProcessedQuantity = item.ProcessedQuantity,
                        WarehouseId = item.WarehouseId,
                        CustomerOrderDetailId = item.CustomerOrderDetailId
                    });
                }

                try
                {
                    await dbContext.CustomerOrdersInProcesses.AddAsync(entity, ct);
                    await dbContext.SaveChangesAsync(ct);

                    return entity;
                }
                catch (Exception)
                {
                    dbContext.Entry(entity).State = EntityState.Unchanged;
                    throw;
                }
            }, ct);
        }

        public async Task<IEnumerable<CustomerOrdersInProcess>> GetByCustomerOrderIdAsync(int customerOrderId, CancellationToken ct)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.CustomerOrdersInProcesses.AsNoTracking()
                            .Include(i => i.CustomerOrder)
                            .Include(i => i.EmployeeRecipient)
                            .Include(i => i.Employee)
                            .Include(i => i.ProcessSatellite)
                            .Include(i => i.StatusDocumentType)
                            .Where(i => i.CustomerOrderId == customerOrderId)
                            .ToListAsync(ct);
            }, ct);
        }

        public async Task UpdateAsync(int customerOrderInProcessId, CustomerOrdersInProcess customerOrdersInProcess, Reason reason, CancellationToken ct)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.CustomerOrdersInProcesses.Include(i => i.CustomerOrderInProcessDetails).FirstOrDefaultAsync(x => x.CustomerOrderInProcessId == customerOrderInProcessId, ct) ?? throw new KeyNotFoundException($"Traslado a proceso con id {customerOrderInProcessId} no existe.");

                entity.TransferDatetime = customerOrdersInProcess.TransferDatetime;
                entity.ProcessDate = customerOrdersInProcess.ProcessDate;
                entity.CustomerOrderId = customerOrdersInProcess.CustomerOrderId;
                entity.EmployeeRecipientId = customerOrdersInProcess.EmployeeRecipientId;
                entity.EmployeeId = customerOrdersInProcess.EmployeeId;
                entity.StatusDocumentTypeId = customerOrdersInProcess.StatusDocumentTypeId;
                entity.Notes = customerOrdersInProcess.Notes;

                foreach (var item in customerOrdersInProcess.CustomerOrderInProcessDetails)
                {
                    if (item.CustomerOrderInProcessDetailId > 0)
                    {
                        var detail = entity.CustomerOrderInProcessDetails.FirstOrDefault(i => i.CustomerOrderInProcessDetailId == item.CustomerOrderInProcessDetailId);
                        if (detail != null)
                        {
                            detail.Brand = item.Brand;
                            detail.WarehouseId = item.WarehouseId;
                            detail.ProcessedQuantity = item.ProcessedQuantity;
                            detail.CustomerOrderDetailId = item.CustomerOrderDetailId;
                        }
                        continue;
                    }

                    entity.CustomerOrderInProcessDetails.Add(new CustomerOrderInProcessDetail()
                    {
                        CustomerOrderInProcessId = item.CustomerOrderInProcessId,
                        CustomerOrderDetailId = item.CustomerOrderDetailId,
                        ProcessedQuantity = item.ProcessedQuantity,
                        Brand = item.Brand,
                        WarehouseId = item.WarehouseId
                    });
                }

                foreach (var item in entity.CustomerOrderInProcessDetails)
                {
                    if (!customerOrdersInProcess.CustomerOrderInProcessDetails.Any(i => i.CustomerOrderInProcessDetailId == item.CustomerOrderInProcessDetailId))
                        dbContext.CustomerOrderInProcessDetails.Remove(item);
                }

                var reasonEntity = new ModifiedOrdersInProcess
                {
                    CustomerOrderInProcessId = customerOrderInProcessId,
                    ModificationReasonId = reason.ReasonId,
                    EmployeeId = reason.EmployeeId,
                    ModificationDate = reason.Date
                };
                try
                {
                    dbContext.ModifiedOrdersInProcesses.Add(reasonEntity);
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

        public async Task<CustomerOrdersInProcess?> FindAsync(int customerOrderInProcessId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.CustomerOrdersInProcesses.AsNoTracking()
               .Include(i => i.CustomerOrder)
               .Include(i => i.EmployeeRecipient)
               .Include(i => i.Employee)
               .Include(i => i.ProcessSatellite)
               .Include(i => i.StatusDocumentType)
               .FirstOrDefaultAsync(i => i.CustomerOrderInProcessId == customerOrderInProcessId, ct);
            }, ct);
        }

        public async Task CancelAsync(int customerOrderInProcessId, short canceledStatusDocumentId, Reason reason, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.CustomerOrdersInProcesses.Include(i => i.CustomerOrderInProcessDetails).FirstOrDefaultAsync(x => x.CustomerOrderInProcessId == customerOrderInProcessId, ct) ?? throw new KeyNotFoundException($"Traslado con id {customerOrderInProcessId} no existe.");
                entity.StatusDocumentTypeId = canceledStatusDocumentId;

                var reasonEntity = new CanceledOrdersInProcess
                {
                    CustomerOrderInProcessId = customerOrderInProcessId,
                    CancellationReasonId = reason.ReasonId,
                    EmployeeId = reason.EmployeeId,
                    CancellationDate = reason.Date
                };
                try
                {
                    dbContext.CanceledOrdersInProcesses.Add(reasonEntity);
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
