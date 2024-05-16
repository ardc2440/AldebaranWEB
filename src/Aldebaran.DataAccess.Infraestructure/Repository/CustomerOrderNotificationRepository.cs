using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Enums;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CustomerOrderNotificationRepository : ICustomerOrderNotificationRepository
    {
        private readonly AldebaranDbContext _context;

        public CustomerOrderNotificationRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddAsync(CustomerOrderNotification customerOrderNotification, CancellationToken ct = default)
        {
            try
            {
                await _context.CustomerOrderNotifications.AddAsync(customerOrderNotification, ct);
                await _context.SaveChangesAsync(ct);
            }
            catch (Exception)
            {
                _context.Entry(customerOrderNotification).State = EntityState.Unchanged;
                throw;
            }
        }

        public async Task<IEnumerable<CustomerOrderNotification>> GetByCustomerOrderId(int customerOrderId, CancellationToken ct = default)
        {
            return await _context.CustomerOrderNotifications.AsNoTracking()
                            .Include(i => i.CustomerOrder.Customer)
                            .Include(i => i.NotificationTemplate)
                            .Where(w => w.CustomerOrderId == customerOrderId)
                            .ToListAsync(ct);
        }

        public async Task UpdateAsync(string notificationId, NotificationStatus status, string errorMessage, CancellationToken ct = default)
        {
            var entity = await _context.CustomerOrderNotifications.FirstOrDefaultAsync(w => w.NotificationId.Equals(notificationId)) ?? throw new KeyNotFoundException($"Notificación de la orden de compra con id {notificationId} no existe.");

            try
            {
                entity.NotificationState = status;
                entity.NotificationSendingErrorMessage = errorMessage;
                entity.NotificationDate = DateTime.Now;
                await _context.SaveChangesAsync(ct);
            }
            catch (Exception)
            {
                _context.Entry(entity).State = EntityState.Unchanged;
                throw;
            }
        }
    }
}
