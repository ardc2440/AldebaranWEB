using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class WarehouseTransferReportRepository : RepositoryBase<AldebaranDbContext>, IWarehouseTransferReportRepository
    {
        public WarehouseTransferReportRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<WarehouseTransferReport>> GetWarehouseTransferReportDataAsync(string filter = "", CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Set<WarehouseTransferReport>().FromSqlRaw($"EXEC SP_GET_WAREHOUSE_TRANSFER_REPORT {filter}").ToListAsync(ct);
            }, ct);
        }
    }
}
