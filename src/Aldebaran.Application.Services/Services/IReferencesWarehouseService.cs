using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IReferencesWarehouseService
    {
        Task<IEnumerable<ReferencesWarehouse>> GetByReferenceIdAsync(int referenceId, CancellationToken ct = default);
        Task<ReferencesWarehouse?> GetByReferenceAndWarehouseIdAsync(int referenceId, short warehouseId, CancellationToken ct = default);
        Task<IEnumerable<ReferencesWarehouse>> GetAllAsync(CancellationToken ct = default);
    }

}
