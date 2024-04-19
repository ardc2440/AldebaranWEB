using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IDocumentTypeService
    {
        Task<DocumentType?> FindByCodeAsync(string code, CancellationToken ct = default);
        DocumentType? FindByCode(string code);
    }

}
