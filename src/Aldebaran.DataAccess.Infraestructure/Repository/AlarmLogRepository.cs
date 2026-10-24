using Aldebaran.DataAccess.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class AlarmLogRepository : RepositoryBase<AldebaranDbContext>, IAlarmLogRepository
    {
        public AlarmLogRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<AlarmLogType>> GetTypesAsync(char context, string roles, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                var parameters = new[]
                {
                    new SqlParameter("@Context", SqlDbType.Char, 1) { Value = context.ToString() },
                    new SqlParameter("@Roles", SqlDbType.NVarChar, -1) { Value = roles }
                };

                return await dbContext.Set<AlarmLogType>()
                    .FromSqlRaw("EXEC dbo.SP_GET_ALARM_LOG_TYPES @Context, @Roles", parameters)
                    .AsNoTracking()
                    .ToListAsync(ct);
            }, ct);
        }

        public async Task<IEnumerable<AlarmLogEntry>> GetAlarmsAsync(string typeCode, DateTime dateFrom, DateTime dateTo, string? searchKey, int? employeeId, int skip, int take, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                var parameters = new[]
                {
                    new SqlParameter("@TypeCode", SqlDbType.VarChar, 30) { Value = typeCode },
                    new SqlParameter("@DateFrom", SqlDbType.Date) { Value = dateFrom.Date },
                    new SqlParameter("@DateTo", SqlDbType.Date) { Value = dateTo.Date },
                    new SqlParameter("@SearchKey", SqlDbType.NVarChar, 100) { Value = (object?)searchKey ?? DBNull.Value },
                    new SqlParameter("@Skip", SqlDbType.Int) { Value = skip },
                    new SqlParameter("@Take", SqlDbType.Int) { Value = take },
                    new SqlParameter("@EmployeeId", SqlDbType.Int) { Value = (object?)employeeId ?? DBNull.Value }
                };

                return await dbContext.Set<AlarmLogEntry>()
                    .FromSqlRaw("EXEC dbo.SP_GET_ALARM_LOG @TypeCode = @TypeCode, @DateFrom = @DateFrom, @DateTo = @DateTo, @SearchKey = @SearchKey, @Skip = @Skip, @Take = @Take, @EmployeeId = @EmployeeId", parameters)
                    .AsNoTracking()
                    .ToListAsync(ct);
            }, ct);
        }

        public async Task<IEnumerable<AlarmLogVisualization>> GetVisualizationsAsync(string typeCode, string alarmKey, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                var parameters = new[]
                {
                    new SqlParameter("@TypeCode", SqlDbType.VarChar, 30) { Value = typeCode },
                    new SqlParameter("@AlarmKey", SqlDbType.VarChar, 50) { Value = alarmKey }
                };

                return await dbContext.Set<AlarmLogVisualization>()
                    .FromSqlRaw("EXEC dbo.SP_GET_ALARM_LOG_VISUALIZATIONS @TypeCode, @AlarmKey", parameters)
                    .AsNoTracking()
                    .ToListAsync(ct);
            }, ct);
        }

        public async Task<IEnumerable<AlarmLogVisualizer>> GetVisualizersAsync(string typeCode, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                var parameters = new[] { new SqlParameter("@TypeCode", SqlDbType.VarChar, 30) { Value = typeCode } };

                return await dbContext.Set<AlarmLogVisualizer>()
                    .FromSqlRaw("EXEC dbo.SP_GET_ALARM_LOG_VISUALIZERS @TypeCode", parameters)
                    .AsNoTracking()
                    .ToListAsync(ct);
            }, ct);
        }

        public async Task<IEnumerable<InventoryAlarmLogEntry>> GetInventoryAlarmLogAsync(DateTime dateFrom, DateTime dateTo, string? alarmTypeCode, string? eventType, string? searchKey, int skip, int take, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                var parameters = new[]
                {
                    new SqlParameter("@DateFrom", SqlDbType.Date) { Value = dateFrom.Date },
                    new SqlParameter("@DateTo", SqlDbType.Date) { Value = dateTo.Date },
                    new SqlParameter("@AlarmTypeCode", SqlDbType.VarChar, 30) { Value = (object?)alarmTypeCode ?? DBNull.Value },
                    new SqlParameter("@EventType", SqlDbType.Char, 1) { Value = (object?)eventType ?? DBNull.Value },
                    new SqlParameter("@SearchKey", SqlDbType.NVarChar, 100) { Value = (object?)searchKey ?? DBNull.Value },
                    new SqlParameter("@Skip", SqlDbType.Int) { Value = skip },
                    new SqlParameter("@Take", SqlDbType.Int) { Value = take }
                };

                return await dbContext.Set<InventoryAlarmLogEntry>()
                    .FromSqlRaw("EXEC dbo.SP_GET_INVENTORY_ALARM_LOG @DateFrom, @DateTo, @AlarmTypeCode, @EventType, @SearchKey, @Skip, @Take", parameters)
                    .AsNoTracking()
                    .ToListAsync(ct);
            }, ct);
        }
    }
}
