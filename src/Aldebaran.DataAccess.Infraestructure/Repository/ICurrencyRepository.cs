using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface ICurrencyRepository
    {
        Task<IEnumerable<Currency>> GetAsync(CancellationToken ct = default);
    }
}
