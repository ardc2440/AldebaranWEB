using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface ICustomerReservationDetailService
    {
        Task<IEnumerable<CustomerReservationDetail>> GetAsync(int customerReservationId, CancellationToken ct = default);

        Task UpdateAsync(int customerReservationDetailId, CustomerReservationDetail customerReservationDetail, CancellationToken ct = default);
    }

}
