using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Infraestructure.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface ICancellationRequestRepository
    {
        Task<bool> ExistsAnyPendingRequestAsync(short documentTypeId, int documentNumber, CancellationToken ct = default);
        Task AddAsync(CancellationRequest cancellationRequest, Reason reason, CancellationToken ct = default);
        Task<(IEnumerable<CancellationRequestModel>,int)> GetAllByStatusOrderAsync(int skip, int top, string? searchKey, bool getPendingRequest, CancellationToken ct = default);
        Task<CancellationRequest> UpdateAsync(CancellationRequest cancellationRequest, CancellationToken ct = default);
        Task<CancellationRequest?> FindAsync(int requestId, CancellationToken ct = default);
    }
}
