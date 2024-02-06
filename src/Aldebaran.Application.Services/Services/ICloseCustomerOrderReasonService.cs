using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface ICloseCustomerOrderReasonService 
    {
        Task<IEnumerable<CloseCustomerOrderReason>> GetAsync(CancellationToken ct = default);
    }
}
