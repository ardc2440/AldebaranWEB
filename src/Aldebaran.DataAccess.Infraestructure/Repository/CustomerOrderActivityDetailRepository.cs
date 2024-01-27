using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CustomerOrderActivityDetailRepository : ICustomerOrderActivityDetailRepository
    {
        private readonly AldebaranDbContext _context;
        public CustomerOrderActivityDetailRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task DeleteAsync(int customerOrderActivityDetailId, CancellationToken ct = default)
        {
            var entity = await _context.CustomerOrderActivityDetails.FirstOrDefaultAsync(i => i.CustomerOrderActivityDetailId == customerOrderActivityDetailId, ct) ?? throw new KeyNotFoundException($"Detalle de Actividad con id {customerOrderActivityDetailId} no existe."); ;

            try
            {
                _context.CustomerOrderActivityDetails.Remove(entity);
                await _context.SaveChangesAsync(ct);
            }
            catch
            {
                _context.Entry(entity).State = EntityState.Unchanged;
                throw;
            }
        }

        public async Task<IEnumerable<CustomerOrderActivityDetail>> GetByCustomerOrderActivityIdAsync(int customerOrderActivityId, CancellationToken ct = default)
        {
            return await _context.CustomerOrderActivityDetails.AsNoTracking()
                .Include(i => i.ActivityType)
                .Include(i => i.ActivityEmployee)
                .Include(i => i.Employee_EmployeeId.IdentityType)
                .Where(i => i.CustomerOrderActivityId.Equals(customerOrderActivityId))
                .ToListAsync(ct);
        }
    }

}
