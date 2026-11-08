using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class WarehouseTransferReportRepository : RepositoryBase<AldebaranDbContext>, IWarehouseTransferReportRepository
    {
        public WarehouseTransferReportRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<WarehouseTransferReport>> GetWarehouseTransferReportDataAsync(WarehouseTransferReportFilter filter, CancellationToken ct = default)
        {
            var command = ReportCommands.WarehouseTransfer(filter ?? new WarehouseTransferReportFilter());
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Set<WarehouseTransferReport>().FromSqlRaw(command.Sql, command.Parameters).AsNoTracking().ToListAsync(ct);
            }, ct);
        }
    }
}
