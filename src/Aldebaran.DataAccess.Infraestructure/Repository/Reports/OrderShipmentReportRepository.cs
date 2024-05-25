using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class OrderShipmentReportRepository : RepositoryBase<AldebaranDbContext>, IOrderShipmentReportRepository
    {
        public OrderShipmentReportRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<OrderShipmentReport>> GetOrderShipmentReportDataAsync(string filter, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Set<OrderShipmentReport>().FromSqlRaw($"EXEC SP_GET_ORDER_SHIPMENT_REPORT {filter}").ToListAsync(ct);
            }, ct);
        }
    }
}
