using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IReferencesWarehouseRepository
    {
        Task<IEnumerable<ReferencesWarehouse>> GetAllAsync(CancellationToken ct = default);
        Task<IEnumerable<ReferencesWarehouse>> GetByReferenceIdAsync(int referenceId, CancellationToken ct = default);
        Task<ReferencesWarehouse?> GetByReferenceAndWarehouseIdAsync(int referenceId, short warehouseId, CancellationToken ct = default);
    }
}
