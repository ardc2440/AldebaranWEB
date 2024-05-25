using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class StatusDocumentTypeRepository : RepositoryBase<AldebaranDbContext>, IStatusDocumentTypeRepository
    {
        public StatusDocumentTypeRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<StatusDocumentType?> FindByDocumentAndOrderAsync(int documentTypeId, int order, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.StatusDocumentTypes.AsNoTracking().FirstOrDefaultAsync(f => f.DocumentTypeId == documentTypeId && f.StatusOrder == order, ct);

            }, ct);
        }
        public async Task<IEnumerable<StatusDocumentType>> GetByDocumentTypeIdAsync(int documentTypeId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.StatusDocumentTypes.AsNoTracking()
                            .Where(f => f.DocumentTypeId == documentTypeId)
                            .ToListAsync(ct);
            }, ct);
        }

        public async Task<StatusDocumentType?> FindAsync(int statusDocumentTypeId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.StatusDocumentTypes.AsNoTracking().FirstOrDefaultAsync(f => f.StatusDocumentTypeId == statusDocumentTypeId, ct);
            }, ct);
        }
    }
}
