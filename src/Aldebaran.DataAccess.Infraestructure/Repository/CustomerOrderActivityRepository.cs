using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CustomerOrderActivityRepository : ICustomerOrderActivityRepository
    {
        private readonly AldebaranDbContext _context;
        public CustomerOrderActivityRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task DeleteAsync(int customerOrderActivityId, CancellationToken ct = default)
        {
            var entity = await _context.CustomerOrderActivities.FirstOrDefaultAsync(i => i.CustomerOrderActivityId == customerOrderActivityId, ct) ?? throw new KeyNotFoundException($"Actividad con id {customerOrderActivityId} no existe."); ;

            try
            {
                _context.CustomerOrderActivities.Remove(entity);
                await _context.SaveChangesAsync(ct);
            }
            catch
            {
                _context.Entry(entity).State = EntityState.Unchanged;
                throw;
            }
        }

        public async Task<IEnumerable<CustomerOrderActivity>> GetByCustomerOrderIdAsync(int customerOrderId, CancellationToken ct = default)
        {
            return await _context.CustomerOrderActivities.AsNoTracking()
                .Include(i => i.Area)
                .Include(i => i.Employee.IdentityType)
                .Where(i => i.CustomerOrderId.Equals(customerOrderId))
                .ToListAsync(ct);
        }

        public async Task AddAsync(CustomerOrderActivity customerOrderActivity, CancellationToken ct = default)
        {
            var entity = new CustomerOrderActivity()
            {
                ActivityDate = customerOrderActivity.ActivityDate,
                CustomerOrderId = customerOrderActivity.CustomerOrderId,
                AreaId = customerOrderActivity.AreaId,
                EmployeeId = customerOrderActivity.EmployeeId,
                Notes = customerOrderActivity.Notes
            };

            foreach (var item in customerOrderActivity.CustomerOrderActivityDetails)
            {
                var entityDetail = new CustomerOrderActivityDetail()
                {
                    ActivityTypeId = item.ActivityTypeId,
                    ActivityEmployeeId = item.ActivityEmployeeId,
                    EmployeeId = item.EmployeeId
                };

                entity.CustomerOrderActivityDetails.Add(entityDetail);
            }

            try
            {
                _context.CustomerOrderActivities.Add(entity);
                await _context.SaveChangesAsync(ct);
            }
            catch
            {
                _context.Entry(entity).State = EntityState.Unchanged;
                throw;
            }
        }

        public async Task<CustomerOrderActivity?> FindAsync(int customerOrderActivityId, CancellationToken ct)
        {
            return await _context.CustomerOrderActivities.AsNoTracking()
                .Include(i => i.Area)
                .Include(i => i.Employee)
                .FirstOrDefaultAsync(i => i.CustomerOrderActivityId == customerOrderActivityId, ct);
        }

        public async Task UpdateAsync(int customerOrderActivityId, CustomerOrderActivity customerOrderActivity, CancellationToken ct = default)
        {
            var entity = await _context.CustomerOrderActivities.Include(i => i.CustomerOrderActivityDetails).FirstOrDefaultAsync(i => i.CustomerOrderActivityId == customerOrderActivityId, ct) ?? throw new KeyNotFoundException($"Actividad con id {customerOrderActivityId} no existe."); ;

            entity.ActivityDate = customerOrderActivity.ActivityDate;
            entity.AreaId = customerOrderActivity.AreaId;
            entity.CustomerOrderId = customerOrderActivity.CustomerOrderId;
            entity.EmployeeId = customerOrderActivity.EmployeeId;

            foreach (var item in customerOrderActivity.CustomerOrderActivityDetails)
            {
                if (item.CustomerOrderActivityDetailId > 0)
                {
                    var entityDetail = entity.CustomerOrderActivityDetails.FirstOrDefault(i => i.CustomerOrderActivityDetailId == item.CustomerOrderActivityDetailId);

                    if (entityDetail != null)
                    {
                        entityDetail.ActivityEmployeeId = item.ActivityEmployeeId;
                        entityDetail.ActivityTypeId = item.ActivityTypeId;
                        entityDetail.EmployeeId = item.EmployeeId;
                        continue;
                    }
                }

                entity.CustomerOrderActivityDetails.Add(new CustomerOrderActivityDetail()
                {
                    ActivityTypeId = item.ActivityTypeId,
                    EmployeeId = item.EmployeeId,
                    ActivityEmployeeId = item.ActivityEmployeeId,
                    CustomerOrderActivityId = item.CustomerOrderActivityId
                });
            }

            foreach (var item in entity.CustomerOrderActivityDetails)
            {
                if (!customerOrderActivity.CustomerOrderActivityDetails.Any(i => i.CustomerOrderActivityDetailId == item.CustomerOrderActivityDetailId))
                    _context.CustomerOrderActivityDetails.Remove(item);
            }

            try
            {
                await _context.SaveChangesAsync(ct);
            }
            catch
            {
                _context.Entry(entity).State = EntityState.Unchanged;
                throw;
            }
        }
    }

}
