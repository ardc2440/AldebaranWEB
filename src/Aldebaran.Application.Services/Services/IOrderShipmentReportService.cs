using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IOrderShipmentReportService
    {
        Task<IEnumerable<OrderShipmentReport>> GetOrderShipmentReportDataAsync(CancellationToken ct = default);
    }
}
