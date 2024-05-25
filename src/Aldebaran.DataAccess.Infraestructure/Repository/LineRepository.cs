using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class LineRepository : RepositoryBase<AldebaranDbContext>, ILineRepository
    {
        public LineRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<Line>> GetAsync(CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Lines.AsNoTracking()
                           .ToListAsync(ct);
            }, ct);
        }
    }
}
