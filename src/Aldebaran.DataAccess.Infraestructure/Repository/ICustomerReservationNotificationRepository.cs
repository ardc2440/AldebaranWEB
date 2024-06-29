using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Enums;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface ICustomerReservationNotificationRepository
    {
        Task<CustomerReservationNotification?> FindAsync(int customerReservationNotificationId, CancellationToken ct = default);
        Task AddAsync(CustomerReservationNotification customerReservationNotification, CancellationToken ct = default);
        Task UpdateAsync(string notificationId, NotificationStatus status, string errorMessage, DateTime date, CancellationToken ct = default);
        Task<IEnumerable<CustomerReservationNotification>> GetByCustomerReservationIdAsync(int customerReservationId, CancellationToken ct = default);
    }
}
