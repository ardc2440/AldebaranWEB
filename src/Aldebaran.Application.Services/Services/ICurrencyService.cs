using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface ICurrencyService
    {
        Task<IEnumerable<Currency>> GetAsync(CancellationToken ct = default);
    }
}
