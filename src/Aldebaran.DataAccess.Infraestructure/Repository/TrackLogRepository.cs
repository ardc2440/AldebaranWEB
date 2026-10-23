using Aldebaran.DataAccess.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class TrackLogRepository : RepositoryBase<AldebaranDbContext>, ITrackLogRepository
    {
        public TrackLogRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<TrackEntityName>> GetEntitiesAsync(CancellationToken ct = default) =>
            await QueryAsync<TrackEntityName>("EXEC dbo.SP_GET_TRACK_ENTITIES", Array.Empty<SqlParameter>(), ct);

        public async Task<IEnumerable<TrackObjectType>> GetObjectTypesAsync(CancellationToken ct = default) =>
            await QueryAsync<TrackObjectType>("EXEC dbo.SP_GET_TRACK_OBJECT_TYPES", Array.Empty<SqlParameter>(), ct);

        public async Task<IEnumerable<TrackObjectMatch>> SearchObjectsAsync(byte objectType, string text, CancellationToken ct = default) =>
            await QueryAsync<TrackObjectMatch>("EXEC dbo.SP_SEARCH_TRACK_OBJECTS @ObjectType, @Text", new[]
            {
                new SqlParameter("@ObjectType", SqlDbType.TinyInt) { Value = objectType },
                new SqlParameter("@Text", SqlDbType.NVarChar, 100) { Value = text }
            }, ct);

        public async Task<IEnumerable<TrackLogEntry>> GetLogAsync(string? entityName, DateTime dateFrom, DateTime dateTo, string? action, string? modifierName, byte? objectType, int? objectId, int skip, int take, CancellationToken ct = default) =>
            await QueryAsync<TrackLogEntry>("EXEC dbo.SP_GET_TRACK_LOG @EntityName, @DateFrom, @DateTo, @Action, @ModifierName, @ObjectType, @ObjectId, @Skip, @Take", new[]
            {
                new SqlParameter("@EntityName", SqlDbType.NVarChar, 200) { Value = (object?)entityName ?? DBNull.Value },
                new SqlParameter("@DateFrom", SqlDbType.Date) { Value = dateFrom.Date },
                new SqlParameter("@DateTo", SqlDbType.Date) { Value = dateTo.Date },
                new SqlParameter("@Action", SqlDbType.VarChar, 10) { Value = (object?)action ?? DBNull.Value },
                new SqlParameter("@ModifierName", SqlDbType.NVarChar, 100) { Value = (object?)modifierName ?? DBNull.Value },
                new SqlParameter("@ObjectType", SqlDbType.TinyInt) { Value = (object?)objectType ?? DBNull.Value },
                new SqlParameter("@ObjectId", SqlDbType.Int) { Value = (object?)objectId ?? DBNull.Value },
                new SqlParameter("@Skip", SqlDbType.Int) { Value = skip },
                new SqlParameter("@Take", SqlDbType.Int) { Value = take }
            }, ct);

        public async Task<TrackLogDetail?> GetDetailAsync(long id, CancellationToken ct = default)
        {
            var rows = await QueryAsync<TrackLogDetail>("EXEC dbo.SP_GET_TRACK_DETAIL @Id", new[] { new SqlParameter("@Id", SqlDbType.BigInt) { Value = id } }, ct);
            return rows.FirstOrDefault();
        }

        public async Task<IEnumerable<TrackObjectDescription>> GetObjectDescriptionsAsync(long id, CancellationToken ct = default) =>
            await QueryAsync<TrackObjectDescription>("EXEC dbo.SP_GET_TRACK_OBJECT_DESCRIPTIONS @Id", new[] { new SqlParameter("@Id", SqlDbType.BigInt) { Value = id } }, ct);

        /// <summary>Ejecuta un SP de solo lectura y materializa su resultado en la entidad keyless indicada.</summary>
        private async Task<List<TRow>> QueryAsync<TRow>(string sql, SqlParameter[] parameters, CancellationToken ct) where TRow : class
        {
            return await ExecuteQueryAsync(async dbContext =>
                await dbContext.Set<TRow>()
                    .FromSqlRaw(sql, parameters)
                    .AsNoTracking()
                    .ToListAsync(ct), ct);
        }
    }
}
