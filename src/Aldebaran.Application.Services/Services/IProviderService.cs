using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IProviderService
    {
        Task<IEnumerable<Provider>> GetAsync(CancellationToken ct = default);
        Task<IEnumerable<Provider>> GetAsync(string searchKey, CancellationToken ct = default);
        Task<Provider?> FindAsync(int providerId, CancellationToken ct = default);
        Task AddAsync(Provider provider, CancellationToken ct = default);
        Task UpdateAsync(int providerId, Provider provider, CancellationToken ct = default);
        Task DeleteAsync(int providerId, CancellationToken ct = default);
    }
}
