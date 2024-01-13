using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CustomerOrdersInProcessRepository : ICustomerOrdersInProcessRepository
    {
        private readonly AldebaranDbContext _context;
        public CustomerOrdersInProcessRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddAsync(CustomerOrdersInProcess customerOrdersInProcess, CancellationToken ct)
        {
            var entity = new CustomerOrdersInProcess
            {
                CustomerOrderId = customerOrdersInProcess.CustomerOrderId,
                EmployeeRecipientId = customerOrdersInProcess.EmployeeRecipientId,
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
                await _context.CustomerOrdersInProcesses.AddAsync(entity, ct);
                await _context.SaveChangesAsync(ct);
            }
            catch (Exception)
            {
                _context.Entry(entity).State = EntityState.Unchanged;
                throw;
            }
            customerOrdersInProcess.CustomerOrderInProcessId = entity.CustomerOrderInProcessId;
        }

        public async Task<IEnumerable<CustomerOrdersInProcess>> GetByCustomerOrderIdAsync(int customerOrderId, CancellationToken ct)
        {
            return await _context.CustomerOrdersInProcesses.AsNoTracking()
                .Include(i => i.CustomerOrder)
                .Include(i => i.Employee)
                .Include(i => i.ProcessSatellite)
                .Include(i => i.StatusDocumentType)
                .Where(i => i.CustomerOrderId == customerOrderId)
                .ToListAsync(ct);
        }

        public async Task UpdateAsync(int customerOrderInProcessId, CustomerOrdersInProcess customerOrdersInProcess, CancellationToken ct)
        {
            var entity = await _context.CustomerOrdersInProcesses.Include(i => i.CustomerOrderInProcessDetails).FirstOrDefaultAsync(x => x.CustomerOrderInProcessId == customerOrderInProcessId, ct) ?? throw new KeyNotFoundException($"Traslado a proceso con id {customerOrderInProcessId} no existe.");

            entity.TransferDatetime = customerOrdersInProcess.TransferDatetime;
            entity.ProcessDate = customerOrdersInProcess.ProcessDate;
            entity.CustomerOrderId = customerOrdersInProcess.CustomerOrderId;
            entity.EmployeeRecipientId = customerOrdersInProcess.EmployeeRecipientId;
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
                if (!customerOrdersInProcess.CustomerOrderInProcessDetails.Any(i => i.CustomerOrderInProcessId == item.CustomerOrderInProcessId))
                    _context.CustomerOrderInProcessDetails.Remove(item);
            }

            try
            {
                await _context.SaveChangesAsync(ct);
            }
            catch (Exception)
            {
                _context.Entry(entity).State = EntityState.Unchanged;
                throw;
            }
        }

        public async Task<CustomerOrdersInProcess?> FindAsync(int customerOrderInProcessId, CancellationToken ct = default)
        {
            return await _context.CustomerOrdersInProcesses.AsNoTracking()
                .Include(i => i.CustomerOrder)
                .Include(i => i.Employee)
                .Include(i => i.ProcessSatellite)
                .Include(i => i.StatusDocumentType)
                .FirstOrDefaultAsync(i => i.CustomerOrderInProcessId == customerOrderInProcessId, ct);
        }
    }
}
