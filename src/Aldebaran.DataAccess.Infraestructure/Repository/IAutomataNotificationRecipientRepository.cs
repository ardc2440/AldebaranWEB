using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IAutomataNotificationRecipientRepository
    {
        Task<AutomataNotificationRecipient> CreateAsync(AutomataNotificationRecipient entity, CancellationToken ct = default);
        Task<AutomataNotificationRecipient> UpdateAsync(AutomataNotificationRecipient entity, CancellationToken ct = default);
        Task<IEnumerable<AutomataNotificationRecipient>> GetAllAsync(CancellationToken ct = default);
        Task<AutomataNotificationRecipient> GetByIdAsync(int id, CancellationToken ct = default);
        Task<List<string>> GetActiveEmailsByTypeAsync(string notificationType, CancellationToken ct = default);
    }
}
