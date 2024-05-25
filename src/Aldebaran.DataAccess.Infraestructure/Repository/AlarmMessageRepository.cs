using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class AlarmMessageRepository : RepositoryBase<AldebaranDbContext>, IAlarmMessageRepository
    {
        public AlarmMessageRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<AlarmMessage>> GetByDocumentTypeIdAsync(short documentTypeId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.AlarmMessages.AsNoTracking()
                    .Include(i => i.AlarmType.DocumentType)
                    .Where(i => i.AlarmType.DocumentTypeId == documentTypeId)
                    .ToListAsync(ct);
            }, ct);
        }
    }

}
