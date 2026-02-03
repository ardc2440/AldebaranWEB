using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services.Services
{
    public interface IAutomataConnectivityErrorPatternService
    {
        Task<AutomataConnectivityErrorPattern> CreateAsync(AutomataConnectivityErrorPattern model, CancellationToken ct = default);
        Task<AutomataConnectivityErrorPattern> UpdateAsync(AutomataConnectivityErrorPattern model, CancellationToken ct = default);
        Task<IEnumerable<AutomataConnectivityErrorPattern>> GetAllAsync(CancellationToken ct = default);
        Task<AutomataConnectivityErrorPattern> GetByIdAsync(int id, CancellationToken ct = default);
    }
}
