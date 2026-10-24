using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IInternalReferenceConsecutiveRepository
    {
        Task<IEnumerable<InternalReferenceConsecutiveEntry>> GetAvailableAsync(string code, CancellationToken ct = default);

        Task<IEnumerable<InternalReferenceConsecutiveEntry>> GenerateAsync(string code, int quantity, CancellationToken ct = default);

        Task<IEnumerable<InternalReferenceNonconformingEntry>> GetNonconformingAsync(string code, CancellationToken ct = default);
    }
}
