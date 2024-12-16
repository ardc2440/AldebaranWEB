using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface ICancellationRequestService
    {
        Task<bool> ExistsAnyPendingRequestAsync(short documentTypeId, int documentNumber, CancellationToken ct = default);
        Task AddAsync(CancellationRequest cancellationRequest, Reason reason, CancellationToken ct = default);
        Task<CancellationRequest> UpdateAsync(CancellationRequest cancellationRequest, CancellationToken ct = default);
        Task<(IEnumerable<CancellationRequestModel>, int)> GetAllByStatusOrderAsync(int skip, int top, string search, string type, bool getPendingRequest, CancellationToken ct = default);
        Task<CancellationRequest?> FindAsync(int requestId, CancellationToken ct = default);
    }
}
