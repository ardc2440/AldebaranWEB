using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CustomerReservationNotificationRepository : ICustomerReservationNotificationRepository
    {
        private readonly AldebaranDbContext _context;
        public CustomerReservationNotificationRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddAsync(CustomerReservationNotification customerReservationNotification, CancellationToken ct = default)
        {
            try
            {
                await _context.CustomerReservationNotifications.AddAsync(customerReservationNotification, ct);
                await _context.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                _context.Entry(customerReservationNotification).State = EntityState.Unchanged;
                throw;
            }
        }

        public async Task<IEnumerable<CustomerReservationNotification>> GetByCustomerReservationIdAsync(int customerReservationId, CancellationToken ct = default)
        {
            return await _context.CustomerReservationNotifications.AsNoTracking()
                            .Include(i => i.CustomerReservation.Customer)
                            .Include(i => i.NotificationTemplate)
                            .Where(w => w.CustomerReservationId == customerReservationId)
                            .ToListAsync(ct);
        }

        public async Task UpdateAsync(string notificationId, NotificationStatus status, string errorMessage, DateTime date, CancellationToken ct = default)
        {
            var entity = await _context.CustomerReservationNotifications.FirstOrDefaultAsync(w => w.NotificationId.Equals(notificationId)) ?? throw new KeyNotFoundException($"Notificación de la orden de compra con id {notificationId} no existe.");

            try
            {
                entity.NotificationState = status;
                entity.NotificationSendingErrorMessage = errorMessage;
                entity.NotificationDate = date;
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
