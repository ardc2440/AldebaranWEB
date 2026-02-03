using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IAutomataConnectivityErrorPatternRepository
    {
        Task<AutomataConnectivityErrorPattern> CreateAsync(AutomataConnectivityErrorPattern entity, CancellationToken ct = default);
        Task<AutomataConnectivityErrorPattern> UpdateAsync(AutomataConnectivityErrorPattern entity, CancellationToken ct = default);
        Task<IEnumerable<AutomataConnectivityErrorPattern>> GetAllAsync(CancellationToken ct = default);
        Task<AutomataConnectivityErrorPattern> GetByIdAsync(int id, CancellationToken ct = default);
    }
}
