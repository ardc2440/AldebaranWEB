using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface ICancellationReasonRepository {
        Task<IEnumerable<CancellationReason>> GetAsync(string documentTypeCode, CancellationToken ct = default);
    }

}
