using Aldebaran.Application.Services.Models.Reports;

namespace Aldebaran.Application.Services.Reports
{
    public interface IOrderShipmentReportService
    {
        Task<IEnumerable<OrderShipmentReport>> GetOrderShipmentReportDataAsync(CancellationToken ct = default);
    }
}
