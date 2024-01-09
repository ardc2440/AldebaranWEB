using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CustomerOrderInProcessDetailRepository : ICustomerOrderInProcessDetailRepository
    {
        private readonly AldebaranDbContext _context;
        public CustomerOrderInProcessDetailRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<CustomerOrderInProcessDetail>> GetByCustomerOrderInProcessIdAsync(int customerOrderInProcessId, CancellationToken ct)
        {
            return await _context.CustomerOrderInProcessDetails.AsNoTracking()
                .Include(i => i.CustomerOrderDetail)
                .Include(i => i.Warehouse)
                .Where(i => i.CustomerOrderInProcessId == customerOrderInProcessId)
                .ToListAsync(ct);
        }
    }

}
