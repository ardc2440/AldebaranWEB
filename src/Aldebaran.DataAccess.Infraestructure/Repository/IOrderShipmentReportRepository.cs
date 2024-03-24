using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IOrderShipmentReportRepository
    {
        Task<IEnumerable<OrderShipmentReport>> GetOrderShipmentReportDataAsync(CancellationToken ct = default);
    }
}
