using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Enums;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CustomerOrderNotificationRepository : RepositoryBase<AldebaranDbContext>, ICustomerOrderNotificationRepository
    {
        public CustomerOrderNotificationRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task AddAsync(CustomerOrderNotification customerOrderNotification, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                try
                {
                    await dbContext.CustomerOrderNotifications.AddAsync(customerOrderNotification, ct);
                    await dbContext.SaveChangesAsync(ct);
                }
                catch (Exception)
                {
                    dbContext.Entry(customerOrderNotification).State = EntityState.Unchanged;
                    throw;
                }
            }, ct);
        }

        public async Task<IEnumerable<CustomerOrderNotification>> GetByCustomerOrderIdAsync(int customerOrderId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.CustomerOrderNotifications.AsNoTracking()
                           .Include(i => i.CustomerOrder.Customer)
                           .Include(i => i.NotificationTemplate)
                           .Where(w => w.CustomerOrderId == customerOrderId)
                           .ToListAsync(ct);
            }, ct);
        }

        public async Task UpdateAsync(string notificationId, NotificationStatus status, string errorMessage, DateTime date, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.CustomerOrderNotifications.FirstOrDefaultAsync(w => w.NotificationId.Equals(notificationId)) ?? throw new KeyNotFoundException($"Notificación de la orden de compra con id {notificationId} no existe.");

                try
                {
                    entity.NotificationState = status;
                    entity.NotificationSendingErrorMessage = errorMessage;
                    entity.NotificationDate = date;
                    await dbContext.SaveChangesAsync(ct);
                }
                catch (Exception)
                {
                    dbContext.Entry(entity).State = EntityState.Unchanged;
                    throw;
                }
            }, ct);
        }
    }
}
