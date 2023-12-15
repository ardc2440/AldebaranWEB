using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IAdjustmentTypeService
    {
        Task<IEnumerable<AdjustmentType>> GetAsync(CancellationToken ct = default);
    }

}
