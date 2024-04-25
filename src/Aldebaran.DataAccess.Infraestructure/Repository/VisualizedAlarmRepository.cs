using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class VisualizedAlarmRepository : IVisualizedAlarmRepository
    {
        private readonly AldebaranDashBoardDbContext _context;
        public VisualizedAlarmRepository(AldebaranDashBoardDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<VisualizedAlarm> AddAsync(VisualizedAlarm item, CancellationToken ct = default)
        {
            await _context.VisualizedAlarms.AddAsync(item, ct);
            await _context.SaveChangesAsync(ct);
            return item;
        }
    }

}
