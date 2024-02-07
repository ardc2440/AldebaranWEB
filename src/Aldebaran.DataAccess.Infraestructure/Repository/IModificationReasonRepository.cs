using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IModificationReasonRepository
    {
        Task<IEnumerable<ModificationReason>> GetAsync(string documentTypeCode, CancellationToken ct = default);
    }
}
