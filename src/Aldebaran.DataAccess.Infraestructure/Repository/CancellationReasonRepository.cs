using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CancellationReasonRepository : RepositoryBase<AldebaranDbContext>, ICancellationReasonRepository
    {
        public CancellationReasonRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<CancellationReason>> GetAsync(string documentTypeCode, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.CancellationReasons.AsNoTracking()
                  .Where(i => i.DocumentType.DocumentTypeCode.Contains(documentTypeCode))
                  .ToListAsync(ct);
            }, ct);
        }
    }

}
