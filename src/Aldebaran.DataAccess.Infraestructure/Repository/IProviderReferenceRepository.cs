using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IProviderReferenceRepository
    {
        Task<ProviderReference?> FindAsync(int providerId, int referenceId, CancellationToken ct = default);
        Task<IEnumerable<ProviderReference>> GetByProviderIdAsync(int providerId, CancellationToken ct = default);
        Task AddRangeAsync(List<ProviderReference> providerReferences, CancellationToken ct = default);
        Task AddAsync(ProviderReference providerReference, CancellationToken ct = default);
        Task DeleteAsync(int providerId, int referenceId, CancellationToken ct = default);
        Task<IEnumerable<ProviderReference>> GetProviderReferecesReport(CancellationToken ct = default);
    }
}
