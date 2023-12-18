using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IIdentityTypeService
    {
        Task<IEnumerable<IdentityType>> GetAsync(CancellationToken ct = default);
    }
}
