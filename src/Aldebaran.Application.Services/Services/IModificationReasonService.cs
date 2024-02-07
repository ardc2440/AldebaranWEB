using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IModificationReasonService
    {
        Task<IEnumerable<ModificationReason>> GetAsync(string documentTypeCode, CancellationToken ct = default);
    }
}
