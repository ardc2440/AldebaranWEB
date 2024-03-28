using Aldebaran.DataAccess.Entities.Reports;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public interface ICustomerReservationReportRepository
    {
        Task<IEnumerable<CustomerReservationReport>> GetCustomerReservationReportDataAsync(string filter = "", CancellationToken ct = default);
    }
}
