using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CustomerOrderShipmentDetailRepository : ICustomerOrderShipmentDetailRepository
    {
        private readonly AldebaranDbContext _context;
        public CustomerOrderShipmentDetailRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<CustomerOrderShipmentDetail>> GetByCustomerOrderShipmentIdAsync(int customerOrderShipmentId, CancellationToken ct)
        {
            return await _context.CustomerOrderShipmentDetails.AsNoTracking()
                .Include(i => i.CustomerOrderDetail.ItemReference.Item.Line)
                .Include(i => i.Warehouse)
                .Where(i => i.CustomerOrderShipmentId == customerOrderShipmentId)
                .ToListAsync(ct);
        }
    }

}
