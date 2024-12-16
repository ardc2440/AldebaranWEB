using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services.Services
{
    public interface IPackagingService
    {
        Task<Packaging?> FindByItemId(int itemId, CancellationToken ct = default);
    }
}
