using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IInventoryReportRepository
    {
        Task<IEnumerable<InventoryReport>> GetInventoryReportDataAsync(CancellationToken ct = default);
    }
}
