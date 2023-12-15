using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class LineRepository : ILineRepository
    {
        private readonly AldebaranDbContext _context;
        public LineRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Line>> GetAsync(CancellationToken ct = default)
        {
            return await _context.Lines.AsNoTracking()
               .ToListAsync(ct);
        }
    }
}
