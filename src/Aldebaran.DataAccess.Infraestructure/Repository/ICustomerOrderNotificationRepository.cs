using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Enums;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface ICustomerOrderNotificationRepository
    {
        Task AddAsync(CustomerOrderNotification customerOrderNotification, CancellationToken ct =default);
        Task UpdateAsync(string notificationId, NotificationStatus status, string errorMessage, CancellationToken ct = default);
        Task<IEnumerable<CustomerOrderNotification>> GetByCustomerOrderId(int customerorderId, CancellationToken ct = default); 
    }
}
