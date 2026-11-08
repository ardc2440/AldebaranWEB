using Aldebaran.Application.Services.Models.Reports;

namespace Aldebaran.Application.Services.Reports
{
    public interface IWarehouseTransferReportService
    {
        Task<IEnumerable<WarehouseTransferReport>> GetWarehouseTransferReportDataAsync(WarehouseTransferReportFilter filter, CancellationToken ct = default);
    }
}
