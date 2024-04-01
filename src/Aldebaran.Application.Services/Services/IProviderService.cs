using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IProviderService
    {
        Task<bool> ExistsByIdentificationNumber(string identificationNumber, CancellationToken ct = default);
        Task<bool> ExistsByCode(string code, CancellationToken ct = default);
        Task<bool> ExistsByName(string name, CancellationToken ct = default);
        Task<IEnumerable<Provider>> GetAsync(CancellationToken ct = default);
        Task<IEnumerable<Provider>> GetAsync(string searchKey, CancellationToken ct = default);
        Task<Provider?> FindAsync(int providerId, CancellationToken ct = default);
        Task AddAsync(Provider provider, CancellationToken ct = default);
        Task UpdateAsync(int providerId, Provider provider, CancellationToken ct = default);
        Task DeleteAsync(int providerId, CancellationToken ct = default);
    }
}
