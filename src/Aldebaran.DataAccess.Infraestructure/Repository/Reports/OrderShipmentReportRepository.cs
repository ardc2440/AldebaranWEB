using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class OrderShipmentReportRepository : IOrderShipmentReportRepository
    {
        private readonly AldebaranDbContext _context;
        public OrderShipmentReportRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<OrderShipmentReport>> GetOrderShipmentReportDataAsync(CancellationToken ct = default)
        {
            return await _context.Set<OrderShipmentReport>().ToListAsync(ct);
        }
    }
}
