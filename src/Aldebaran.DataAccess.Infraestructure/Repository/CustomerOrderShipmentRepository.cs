using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CustomerOrderShipmentRepository : ICustomerOrderShipmentRepository
    {
        private readonly AldebaranDbContext _context;
        public CustomerOrderShipmentRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddAsync(CustomerOrderShipment customerOrderShipment, CancellationToken ct)
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
                    CustomerOrderShipmentId = item.CustomerOrderShipmentId,
                    DeliveredQuantity = item.DeliveredQuantity,
                    WarehouseId = item.WarehouseId,
                    CustomerOrderDetailId = item.CustomerOrderDetailId
                });
            }

            try
            {
                await _context.CustomerOrderShipments.AddAsync(entity, ct);
                await _context.SaveChangesAsync(ct);
            }
            catch (Exception)
            {
                _context.Entry(entity).State = EntityState.Unchanged;
                throw;
            }

            customerOrderShipment.CustomerOrderShipmentId = entity.CustomerOrderShipmentId;
        }

        public async Task<IEnumerable<CustomerOrderShipment>> GetByCustomerOrderIdAsync(int customerOrderId, CancellationToken ct)
        {
            return await _context.CustomerOrderShipments.AsNoTracking()
                .Include(i => i.CustomerOrder)
                .Include(i => i.Employee)
                .Include(i => i.StatusDocumentType)
                .Where(i => i.CustomerOrderId == customerOrderId)
                .ToListAsync(ct);
        }

        public async Task UpdateAsync(int customerOrderShipmentId, CustomerOrderShipment customerOrderShipment, CancellationToken ct)
        {
            var entity = await _context.CustomerOrderShipments.Include(i => i.CustomerOrderShipmentDetails).FirstOrDefaultAsync(x => x.CustomerOrderShipmentId == customerOrderShipmentId, ct) ?? throw new KeyNotFoundException($"Despacho de pedido con id {customerOrderShipmentId} no existe.");

            entity.ShippingDate = customerOrderShipment.ShippingDate;
            entity.CreationDate = customerOrderShipment.CreationDate;
            entity.CustomerOrderId = customerOrderShipment.CustomerOrderId;
            entity.EmployeeId = customerOrderShipment.EmployeeId;
            entity.StatusDocumentTypeId = customerOrderShipment.StatusDocumentTypeId;
            entity.Notes = customerOrderShipment.Notes;
            entity.ShippingMethodId = customerOrderShipment.ShippingMethodId;
            entity.DeliveryNote = customerOrderShipment.DeliveryNote;
            entity.TrackingNumber = customerOrderShipment.TrackingNumber;

            foreach (var item in customerOrderShipment.CustomerOrderShipmentDetails)
            {
                if (item.CustomerOrderShipmentDetailId > 0)
                {
                    var detail = entity.CustomerOrderShipmentDetails.FirstOrDefault(i => i.CustomerOrderShipmentDetailId == item.CustomerOrderShipmentDetailId);
                    if (detail != null)
                    {
                        detail.WarehouseId = item.WarehouseId;
                        detail.DeliveredQuantity = item.DeliveredQuantity;
                        detail.CustomerOrderDetailId = item.CustomerOrderDetailId;
                    }
                    continue;
                }

                entity.CustomerOrderShipmentDetails.Add(new CustomerOrderShipmentDetail()
                {
                    CustomerOrderShipmentId = item.CustomerOrderShipmentId,
                    CustomerOrderDetailId = item.CustomerOrderDetailId,
                    DeliveredQuantity = item.DeliveredQuantity,
                    WarehouseId = item.WarehouseId
                });
            }

            foreach (var item in entity.CustomerOrderShipmentDetails)
            {
                if (!customerOrderShipment.CustomerOrderShipmentDetails.Any(i => i.CustomerOrderShipmentId == item.CustomerOrderShipmentId))
                    _context.CustomerOrderShipmentDetails.Remove(item);
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

        public async Task<CustomerOrderShipment?> FindAsync(int customerOrderShipmentId, CancellationToken ct = default)
        {
            return await _context.CustomerOrderShipments.AsNoTracking()
                .Include(i => i.CustomerOrder)
                .Include(i => i.Employee)
                .Include(i => i.ShippingMethod)
                .Include(i => i.StatusDocumentType)
                .FirstOrDefaultAsync(i => i.CustomerOrderShipmentId == customerOrderShipmentId, ct);
        }
    }

}
