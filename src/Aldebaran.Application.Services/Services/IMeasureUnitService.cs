using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IMeasureUnitService
    {
        Task<IEnumerable<MeasureUnit>> GetAsync(CancellationToken ct = default);
    }
}
