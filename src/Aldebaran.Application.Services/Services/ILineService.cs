using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface ILineService
    {
        Task<IEnumerable<Line>> GetAsync(CancellationToken ct = default);
    }
}
