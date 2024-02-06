using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface ICloseCustomerOrderReasonRepository 
    {
        Task<IEnumerable<CloseCustomerOrderReason>> GetAsync(CancellationToken ct = default);
    }
}
