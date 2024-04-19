using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IDocumentTypeRepository
    {
        Task<DocumentType?> FindByCodeAsync(string code, CancellationToken ct = default);
    }

}
