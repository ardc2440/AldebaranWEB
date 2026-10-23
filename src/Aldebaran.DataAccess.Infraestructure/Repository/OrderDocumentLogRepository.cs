using Aldebaran.DataAccess.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class OrderDocumentLogRepository : RepositoryBase<AldebaranDbContext>, IOrderDocumentLogRepository
    {
        public OrderDocumentLogRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<OrderDocumentLogEntry>> GetLogAsync(string logType, DateTime dateFrom, DateTime dateTo, string? searchKey, int skip, int take, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                var parameters = new[]
                {
                    new SqlParameter("@LogType", SqlDbType.VarChar, 30) { Value = logType },
                    new SqlParameter("@DateFrom", SqlDbType.Date) { Value = dateFrom.Date },
                    new SqlParameter("@DateTo", SqlDbType.Date) { Value = dateTo.Date },
                    new SqlParameter("@SearchKey", SqlDbType.NVarChar, 100) { Value = (object?)searchKey ?? DBNull.Value },
                    new SqlParameter("@Skip", SqlDbType.Int) { Value = skip },
                    new SqlParameter("@Take", SqlDbType.Int) { Value = take }
                };

                return await dbContext.Set<OrderDocumentLogEntry>()
                    .FromSqlRaw("EXEC dbo.SP_GET_ORDER_DOCUMENT_LOG @LogType, @DateFrom, @DateTo, @SearchKey, @Skip, @Take", parameters)
                    .AsNoTracking()
                    .ToListAsync(ct);
            }, ct);
        }
    }
}
