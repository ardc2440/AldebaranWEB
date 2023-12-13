using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface ICountryRepository
    {
        Task<IEnumerable<Country>> GetAsync(CancellationToken ct = default);
        Task<Country?> FindAsync(int countryId, CancellationToken ct = default);
    }
}
