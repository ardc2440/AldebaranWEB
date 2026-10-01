using Aldebaran.DataAccess.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ArticleInventoryRepository : RepositoryBase<AldebaranDbContext>, IArticleInventoryRepository
    {
        public ArticleInventoryRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<ItemReferenceInventory>> GetByReferenceAsync(int referenceId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                var referenceIdParameter = new SqlParameter("@ReferenceId", SqlDbType.Int) { Value = referenceId };

                return await dbContext.Set<ItemReferenceInventory>()
                    .FromSqlRaw("EXEC dbo.SP_GET_ITEM_REFERENCES_INVENTORY @ReferenceId", referenceIdParameter)
                    .AsNoTracking()
                    .ToListAsync(ct);
            }, ct);
        }
    }
}
