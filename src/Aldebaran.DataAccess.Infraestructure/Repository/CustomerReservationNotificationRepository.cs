using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Enums;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CustomerReservationNotificationRepository : RepositoryBase<AldebaranDbContext>, ICustomerReservationNotificationRepository
    {
        public CustomerReservationNotificationRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<CustomerReservationNotification?> FindAsync(int customerReservationNotificationId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.CustomerReservationNotifications.AsNoTracking()
                                .Include(i => i.CustomerReservation.Customer)
                                .Include(i => i.NotificationTemplate)
                                .Where(w => w.CustomerReservationNotificationId == customerReservationNotificationId)
                                .FirstOrDefaultAsync(ct);
            }, ct);
        }

        public async Task AddAsync(CustomerReservationNotification customerReservationNotification, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                try
                {
                    await dbContext.CustomerReservationNotifications.AddAsync(customerReservationNotification, ct);
                    await dbContext.SaveChangesAsync(ct);
                }
                catch (Exception ex)
                {
                    dbContext.Entry(customerReservationNotification).State = EntityState.Unchanged;
                    throw;
                }
            }, ct);
        }

        public async Task<IEnumerable<CustomerReservationNotification>> GetByCustomerReservationIdAsync(int customerReservationId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.CustomerReservationNotifications.AsNoTracking()
                                        .Include(i => i.CustomerReservation.Customer)
                                        .Include(i => i.NotificationTemplate)
                                        .Where(w => w.CustomerReservationId == customerReservationId)
                                        .ToListAsync(ct);
            }, ct);
        }

        public async Task UpdateAsync(string notificationId, NotificationStatus status, string errorMessage, DateTime date, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.CustomerReservationNotifications.FirstOrDefaultAsync(w => w.NotificationId.Equals(notificationId)) ?? throw new KeyNotFoundException($"Notificación de la orden de compra con id {notificationId} no existe.");

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
