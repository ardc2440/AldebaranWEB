using Aldebaran.DataAccess.Entities.Reports;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public interface IWarehouseTransferReportRepository
    {
        Task<IEnumerable<WarehouseTransferReport>> GetWarehouseTransferReportDataAsync(WarehouseTransferReportFilter filter, CancellationToken ct = default);
    }
}
