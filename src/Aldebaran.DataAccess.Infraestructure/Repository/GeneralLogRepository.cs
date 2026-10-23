using Aldebaran.DataAccess.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class GeneralLogRepository : RepositoryBase<AldebaranDbContext>, IGeneralLogRepository
    {
        public GeneralLogRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<GeneralLogGroupEntry>> GetGroupsAsync(DateTime dateFrom, DateTime dateTo, string? level, string? source, string? searchKey, int skip, int take, CancellationToken ct = default)
        {
            var parameters = new[]
            {
                DateParameter("@DateFrom", dateFrom), DateParameter("@DateTo", dateTo),
                TextParameter("@Level", 20, level), TextParameter("@Source", 200, source), TextParameter("@SearchKey", 100, searchKey),
                IntParameter("@Skip", skip), IntParameter("@Take", take)
            };
            return await QueryAsync<GeneralLogGroupEntry>("EXEC dbo.SP_GET_GENERAL_LOG_GROUPS @DateFrom, @DateTo, @Level, @Source, @SearchKey, @Skip, @Take", parameters, ct);
        }

        public async Task<IEnumerable<GeneralLogOccurrenceEntry>> GetOccurrencesAsync(string? groupKey, DateTime dateFrom, DateTime dateTo, string? level, string? source, int skip, int take, CancellationToken ct = default)
        {
            var parameters = new[]
            {
                new SqlParameter("@GroupKey", SqlDbType.VarChar, 64) { Value = (object?)groupKey ?? DBNull.Value },
                DateParameter("@DateFrom", dateFrom), DateParameter("@DateTo", dateTo),
                TextParameter("@Level", 20, level), TextParameter("@Source", 200, source),
                IntParameter("@Skip", skip), IntParameter("@Take", take)
            };
            return await QueryAsync<GeneralLogOccurrenceEntry>("EXEC dbo.SP_GET_GENERAL_LOG_OCCURRENCES @GroupKey, @DateFrom, @DateTo, @Level, @Source, @Skip, @Take", parameters, ct);
        }

        public async Task<GeneralLogDetailEntry?> GetDetailAsync(long logId, CancellationToken ct = default)
        {
            var parameters = new[] { new SqlParameter("@LogId", SqlDbType.BigInt) { Value = logId } };
            var rows = await QueryAsync<GeneralLogDetailEntry>("EXEC dbo.SP_GET_GENERAL_LOG_DETAIL @LogId", parameters, ct);
            return rows.FirstOrDefault();
        }

        public async Task<IEnumerable<string>> GetSourcesAsync(CancellationToken ct = default)
        {
            var rows = await QueryAsync<GeneralLogSourceEntry>("EXEC dbo.SP_GET_GENERAL_LOG_SOURCES", Array.Empty<SqlParameter>(), ct);
            return rows.Select(x => x.Source).ToList();
        }

        private async Task<IEnumerable<T>> QueryAsync<T>(string sql, SqlParameter[] parameters, CancellationToken ct) where T : class =>
            await ExecuteQueryAsync(async dbContext =>
                await dbContext.Set<T>().FromSqlRaw(sql, parameters).AsNoTracking().ToListAsync(ct), ct);

        private static SqlParameter DateParameter(string name, DateTime value) => new(name, SqlDbType.Date) { Value = value.Date };

        private static SqlParameter IntParameter(string name, int value) => new(name, SqlDbType.Int) { Value = value };

        private static SqlParameter TextParameter(string name, int size, string? value) =>
            new(name, SqlDbType.NVarChar, size) { Value = (object?)value ?? DBNull.Value };
    }
}
