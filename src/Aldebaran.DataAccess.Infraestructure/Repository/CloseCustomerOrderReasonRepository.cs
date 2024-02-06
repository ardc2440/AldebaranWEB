using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CloseCustomerOrderReasonRepository : ICloseCustomerOrderReasonRepository
    {
        private readonly AldebaranDbContext _context;
        public CloseCustomerOrderReasonRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<CloseCustomerOrderReason>> GetAsync(CancellationToken ct = default)
        {
            return await _context.CloseCustomerOrderReasons.ToListAsync(ct);
        }
    }

}
