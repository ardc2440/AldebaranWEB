using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IStatusDocumentTypeRepository
    {
        Task<StatusDocumentType?> FindByDocumentAndOrderAsync(int documentTypeId, int order, CancellationToken ct = default);
        Task<IEnumerable<StatusDocumentType>> GetByDocumentTypeIdAsync(int documentTypeId, CancellationToken ct = default);
        Task<StatusDocumentType?> FindAsync(int statusDocumentTypeId, CancellationToken ct = default);

    }
}
