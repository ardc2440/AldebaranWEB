using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class DocumentTypeRepository : RepositoryBase<AldebaranDbContext>, IDocumentTypeRepository
    {
        public DocumentTypeRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<DocumentType?> FindByCodeAsync(string code, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.DocumentTypes.AsNoTracking().FirstOrDefaultAsync(f => f.DocumentTypeCode == code, ct);
            }, ct);
        }
    }
}
