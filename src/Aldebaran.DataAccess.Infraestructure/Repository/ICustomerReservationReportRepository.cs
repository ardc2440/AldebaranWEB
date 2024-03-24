using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface ICustomerReservationReportRepository
    {
        Task<IEnumerable<CustomerReservationReport>> GetCustomerReservationReportDataAsync(CancellationToken ct = default);
    }
}
