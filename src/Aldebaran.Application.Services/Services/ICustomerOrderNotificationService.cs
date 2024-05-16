using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface ICustomerOrderNotificationService
    {
        Task AddAsync(CustomerOrderNotification customerOrderNotification, CancellationToken ct = default);
        Task UpdateAsync(string notificationId, NotificationStatus status, string errorMessage, CancellationToken ct = default);
        Task<IEnumerable<CustomerOrderNotification>> GetByCustomerOrderId(int customerOrderId, CancellationToken ct = default);
    }
}
