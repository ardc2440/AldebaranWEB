using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IStatusDocumentTypeService
    {
        Task<StatusDocumentType?> FindByDocumentAndOrderAsync(int documentTypeId, int order, CancellationToken ct = default);
    }

}
