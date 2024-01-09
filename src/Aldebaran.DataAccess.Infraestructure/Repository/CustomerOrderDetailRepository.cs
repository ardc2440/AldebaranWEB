using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CustomerOrderDetailRepository : ICustomerOrderDetailRepository
    {
        private readonly AldebaranDbContext _context;
        public CustomerOrderDetailRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<CustomerOrderDetail>> GetByCustomerOrderIdAsync(int customerOrderId, CancellationToken ct = default)
        {
            return await _context.CustomerOrderDetails.AsNoTracking()
                    .Include(i => i.ItemReference.Item.Line)
                    .Where(i => i.CustomerOrderId == customerOrderId)
                    .ToListAsync(ct);
        }
    }

}
