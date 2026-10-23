using Aldebaran.DataAccess.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class PurchaseOrderLogRepository : RepositoryBase<AldebaranDbContext>, IPurchaseOrderLogRepository
    {
        private const string Arguments = " @DateFrom, @DateTo, @SearchKey, @Skip, @Take";

        public PurchaseOrderLogRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<PurchaseOrderAdjustmentRequestEntry>> GetAdjustmentRequestsAsync(DateTime dateFrom, DateTime dateTo, string? searchKey, int skip, int take, CancellationToken ct = default) =>
            await QueryAsync<PurchaseOrderAdjustmentRequestEntry>("SP_GET_PURCHASE_ORDER_ADJUSTMENT_LOG", dateFrom, dateTo, searchKey, skip, take, ct);

        public async Task<IEnumerable<ApprovalRangeLogEntry>> GetApprovalRangeChangesAsync(DateTime dateFrom, DateTime dateTo, string? searchKey, int skip, int take, CancellationToken ct = default) =>
            await QueryAsync<ApprovalRangeLogEntry>("SP_GET_APPROVAL_RANGE_LOG", dateFrom, dateTo, searchKey, skip, take, ct);

        private async Task<IEnumerable<T>> QueryAsync<T>(string procedure, DateTime dateFrom, DateTime dateTo, string? searchKey, int skip, int take, CancellationToken ct) where T : class
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                var parameters = new[]
                {
                    new SqlParameter("@DateFrom", SqlDbType.Date) { Value = dateFrom.Date },
                    new SqlParameter("@DateTo", SqlDbType.Date) { Value = dateTo.Date },
                    new SqlParameter("@SearchKey", SqlDbType.NVarChar, 100) { Value = (object?)searchKey ?? DBNull.Value },
                    new SqlParameter("@Skip", SqlDbType.Int) { Value = skip },
                    new SqlParameter("@Take", SqlDbType.Int) { Value = take }
                };

                return await dbContext.Set<T>()
                    .FromSqlRaw("EXEC dbo." + procedure + Arguments, parameters)
                    .AsNoTracking()
                    .ToListAsync(ct);
            }, ct);
        }
    }
}
