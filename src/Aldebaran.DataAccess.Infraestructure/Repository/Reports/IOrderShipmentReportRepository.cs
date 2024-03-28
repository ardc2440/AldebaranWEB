using Aldebaran.DataAccess.Entities.Reports;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public interface IOrderShipmentReportRepository
    {
        Task<IEnumerable<OrderShipmentReport>> GetOrderShipmentReportDataAsync(string filter = "", CancellationToken ct = default);
    }
}
