using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface ICancellationReasonService
    {
        Task<IEnumerable<CancellationReason>> GetAsync(string documentTypeCode, CancellationToken ct = default);
    }
}
