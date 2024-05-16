using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Enums;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface ICustomerReservationNotificationRepository
    {
        Task AddAsync(CustomerReservationNotification customerReservationNotification, CancellationToken ct = default);
        Task UpdateAsync(string notificationId, NotificationStatus status, string errorMessage, CancellationToken ct = default);
        Task<IEnumerable<CustomerReservationNotification>> GetByCustomerReservationId(int customerReservationId, CancellationToken ct = default);
    }
}
