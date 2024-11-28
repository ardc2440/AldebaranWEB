using Aldebaran.Application.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.Application.Services.Services
{
    public interface ICancellationRequestService
    {
        public Task<bool> ExistsAnyPendingRequestAsync(short documentTypeId, int documentNumber, CancellationToken ct = default);

        public Task AddAsync(CancellationRequest cancellationRequest, Reason reason, CancellationToken ct = default);
    }
}
