using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CustomerOrderRepository : ICustomerOrderRepository
    {
        private readonly AldebaranDbContext _context;
        public CustomerOrderRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<CustomerOrder> AddAsync(CustomerOrder customerOrder, CancellationToken ct = default)
        {
            await _context.CustomerOrders.AddAsync(customerOrder, ct);
            await _context.SaveChangesAsync(ct);
            return customerOrder;
        }

        public async Task<IEnumerable<CustomerOrder>> GetAsync(CancellationToken ct = default)
        {
            return await _context.CustomerOrders.AsNoTracking()
                .Include(i => i.Customer.City.Department.Country)
                .Include(i => i.Customer.IdentityType)
                .Include(i => i.StatusDocumentType.DocumentType)
                .Include(i => i.Employee.IdentityType)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<CustomerOrder>> GetAsync(string searchKey, CancellationToken ct = default)
        {
            return await _context.CustomerOrders.AsNoTracking()
                .Include(i => i.Customer.City.Department.Country)
                .Include(i => i.Customer.IdentityType)
                .Include(i => i.StatusDocumentType.DocumentType)
                .Include(i => i.Employee.IdentityType)
                .Where(i => i.InternalNotes.Contains(searchKey) ||
                            i.CustomerNotes.Contains(searchKey) ||
                            i.OrderDate.ToString().Contains(searchKey) ||
                            i.CreationDate.ToString().Contains(searchKey) ||
                            i.EstimatedDeliveryDate.ToString().Contains(searchKey) ||
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
                            i.Customer.Email1.Contains(searchKey) ||
                            i.Customer.Email2.Contains(searchKey) ||
                            i.Customer.Email3.Contains(searchKey) ||
                            i.Customer.CustomerAddress.Contains(searchKey) ||
                            i.Customer.Phone1.Contains(searchKey) ||
                            i.Customer.Phone2.Contains(searchKey) ||
                            i.Customer.Fax.Contains(searchKey) ||
                            i.Customer.IdentityType.IdentityTypeCode.Contains(searchKey) ||
                            i.Customer.IdentityType.IdentityTypeName.Contains(searchKey) ||
                            i.Customer.IdentityNumber.Contains(searchKey))
                .ToListAsync(ct);
        }

        public async Task<CustomerOrder?> FindAsync(int customerOrderId, CancellationToken ct = default)
        {
            return await _context.CustomerOrders.AsNoTracking()
                .Include(i => i.Customer.City.Department.Country)
                .Include(i => i.Customer.IdentityType)
                .Include(i => i.StatusDocumentType.DocumentType)
                .Include(i => i.Employee.IdentityType)
                .FirstOrDefaultAsync(i => i.CustomerOrderId.Equals(customerOrderId), ct);
        }

        public async Task UpdateAsync(int customerOrderId, CustomerOrder customerOrder, CancellationToken ct = default)
        {
            var entity = await _context.CustomerOrders.FirstOrDefaultAsync(x => x.CustomerOrderId == customerOrderId, ct) ?? throw new KeyNotFoundException($"Pedido con id {customerOrderId} no existe.");
            entity.OrderDate = customerOrder.OrderDate;
            entity.EstimatedDeliveryDate = customerOrder.EstimatedDeliveryDate;
            entity.CustomerId = customerOrder.CustomerId;
            entity.InternalNotes = customerOrder.InternalNotes;
            entity.CustomerNotes = customerOrder.CustomerNotes;

            var entityDetail = await _context.CustomerOrderDetails.Where(x => x.CustomerOrderId.Equals(customerOrderId)).ToListAsync(ct);

            foreach (var item in entityDetail)
            {
                if (!customerOrder.CustomerOrderDetails.Any(i => i.CustomerOrderDetailId.Equals(item.CustomerOrderDetailId)))
                    entityDetail.Remove(item);
            }

            foreach (var item in entity.CustomerOrderDetails)
            {
                if (item.CustomerOrderDetailId.Equals(0))
                {
                    entityDetail.Add(new CustomerOrderDetail()
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

                var detail = entityDetail.FirstOrDefault(i => i.CustomerOrderDetailId.Equals(item.CustomerOrderDetailId));

                if (detail != null)
                {
                    detail.Brand = item.Brand;
                    detail.ReferenceId = item.ReferenceId;
                    detail.RequestedQuantity = item.RequestedQuantity;
                    detail.ProcessedQuantity = item.ProcessedQuantity;
                    detail.DeliveredQuantity = item.DeliveredQuantity;
                }
            }

            entity.CustomerOrderDetails = entityDetail;

            await _context.SaveChangesAsync(ct);
        }

        public async Task CancelAsync(int customerOrderId, short canceledStatusDocumentId, CancellationToken ct = default)
        {
            var entity = await _context.CustomerOrders.FirstOrDefaultAsync(x => x.CustomerOrderId == customerOrderId, ct) ?? throw new KeyNotFoundException($"Pedido con id {customerOrderId} no existe.");

            entity.StatusDocumentTypeId = canceledStatusDocumentId;

            await _context.SaveChangesAsync(ct);
        }
    }

}
