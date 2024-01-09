using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IReferencesWarehouseService
    {
        Task<IEnumerable<ReferencesWarehouse>> GetByReferenceIdAsync(int referenceId, CancellationToken ct = default);
    }

}
