using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services.Services
{
    public interface IAutomataNotificationRecipientService
    {
        Task<AutomataNotificationRecipient> CreateAsync(AutomataNotificationRecipient model, CancellationToken ct = default);
        Task<AutomataNotificationRecipient> UpdateAsync(AutomataNotificationRecipient model, CancellationToken ct = default);
        Task<IEnumerable<AutomataNotificationRecipient>> GetAllAsync(CancellationToken ct = default);
        Task<AutomataNotificationRecipient> GetByIdAsync(int id, CancellationToken ct = default);
    }
}
