using Aldebaran.Application.Services.Models;


namespace Aldebaran.Application.Services
{
    public interface ICustomerReservationNotificationService
    {
        Task AddAsync(CustomerReservationNotification customerReservationNotification, CancellationToken ct = default);
        Task UpdateAsync(string notificationId, NotificationStatus status, string errorMessage, CancellationToken ct = default);
        Task<IEnumerable<CustomerReservationNotification>> GetByCustomerReservationId(int customerReservationId, CancellationToken ct = default);
    }
}
