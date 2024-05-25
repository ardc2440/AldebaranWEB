using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ModificationReasonRepository : RepositoryBase<AldebaranDbContext>, IModificationReasonRepository
    {
        public ModificationReasonRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<ModificationReason>> GetAsync(string documentTypeCode, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.ModificationReasons.AsNoTracking()
             .Where(i => i.DocumentType.DocumentTypeCode.Contains(documentTypeCode))
             .ToListAsync(ct);
            }, ct);
        }
    }
}
