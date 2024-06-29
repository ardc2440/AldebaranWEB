using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Enums;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface ICustomerOrderNotificationRepository
    {
        Task<CustomerOrderNotification?> FindAsync(int customerOrderNotificationId, CancellationToken ct = default);
        Task AddAsync(CustomerOrderNotification customerOrderNotification, CancellationToken ct =default);
        Task UpdateAsync(string notificationId, NotificationStatus status, string errorMessage, DateTime date, CancellationToken ct = default);
        Task<IEnumerable<CustomerOrderNotification>> GetByCustomerOrderIdAsync(int customerorderId, CancellationToken ct = default); 
    }
}
