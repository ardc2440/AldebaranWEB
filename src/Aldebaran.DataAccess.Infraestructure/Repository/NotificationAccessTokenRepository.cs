using Aldebaran.DataAccess.Entities;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class NotificationAccessTokenRepository : RepositoryBase<AldebaranDbContext>, INotificationAccessTokenRepository
    {
        public NotificationAccessTokenRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<bool> AddAsync(NotificationAccessToken token, CancellationToken ct = default)
        {
            return await ExecuteCommandAsync(async dbContext =>
            {
                await dbContext.NotificationAccessTokens.AddAsync(token, ct);

                try
                {
                    var affectedRows = await dbContext.SaveChangesAsync(ct);
                    return affectedRows > 0;
                }
                catch (Exception)
                {
                    dbContext.Entry(token).State = EntityState.Detached;
                    throw;
                }
            }, ct);
        }

        public async Task<NotificationAccessToken?> FindAsync(Guid tokenId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.NotificationAccessTokens.AsNoTracking()
                            .Include(i => i.NotificationTemplate)
                            .FirstOrDefaultAsync(w => w.NotificationAccessTokenId == tokenId, ct);
            }, ct);
        }

        public async Task<bool> ConsumeAsync(Guid tokenId, CancellationToken ct = default)
        {

            return await ExecuteCommandAsync(async dbContext =>
             {
                 var token = await dbContext.NotificationAccessTokens.FirstOrDefaultAsync(x => x.NotificationAccessTokenId == tokenId, ct);

                 if (token == null) return false;

                 token.IsConsumed = true;
                 token.ConsumedDate = DateTime.Now;

                 try
                 {
                     var affectedRows = await dbContext.SaveChangesAsync(ct);
                     return affectedRows > 0;
                 }
                 catch (Exception)
                 {
                     dbContext.Entry(token).State = EntityState.Unchanged;
                     throw;
                 }
             }, ct);
        }
    }
}
