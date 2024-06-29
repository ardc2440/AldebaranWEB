using Aldebaran.Application.Services.Models;


namespace Aldebaran.Application.Services
{
    public interface ICustomerReservationNotificationService
    {
        Task<CustomerReservationNotification?> FindAsync(int customerReservationNotificationId, CancellationToken ct = default);
        Task AddAsync(CustomerReservationNotification customerReservationNotification, CancellationToken ct = default);
        Task UpdateAsync(string notificationId, NotificationStatus status, string errorMessage, DateTime date, CancellationToken ct = default);
        Task<IEnumerable<CustomerReservationNotification>> GetByCustomerReservationIdAsync(int customerReservationId, CancellationToken ct = default);
    }
}
