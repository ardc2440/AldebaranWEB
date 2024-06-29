using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface ICustomerOrderNotificationService
    {
        Task<CustomerOrderNotification?> FindAsync(int customerOrderNotificationId, CancellationToken ct = default);
        Task AddAsync(CustomerOrderNotification customerOrderNotification, CancellationToken ct = default);
        Task UpdateAsync(string notificationId, NotificationStatus status, string errorMessage, DateTime date, CancellationToken ct = default);
        Task<IEnumerable<CustomerOrderNotification>> GetByCustomerOrderIdAsync(int customerOrderId, CancellationToken ct = default);
    }
}
