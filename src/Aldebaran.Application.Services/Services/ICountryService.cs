using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface ICountryService
    {
        Task<IEnumerable<Country>> GetAsync(CancellationToken ct = default);
        Task<Country?> FindAsync(int countryId, CancellationToken ct = default);
    }
}
