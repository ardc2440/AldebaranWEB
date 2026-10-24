using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class ReferenceMovementReportRepository : RepositoryBase<AldebaranDbContext>, IReferenceMovementReportRepository
    {
        public ReferenceMovementReportRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<ReferenceMovementReport>> GetReferenceMovementReportDataAsync(ReferenceMovementReportFilter filter, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(filter);
            var command = BuildCommand(filter);
            return await ExecuteQueryAsync(async dbContext =>
                await dbContext.Set<ReferenceMovementReport>().FromSqlRaw(command.Sql, command.Parameters).ToListAsync(ct), ct);
        }

        /// <summary>Las fechas se envían sin hora (igual que antes, 'yyyyMMdd'); condiciones Sí/No/Todos (null no se envía = Todos).</summary>
        private static StoredProcedureCommand BuildCommand(ReferenceMovementReportFilter filter) =>
            new StoredProcedureCommand("SP_GET_REFERENCE_MOVEMENT_REPORT")
                .VarChar("@ReferenceIds", filter.ReferenceIds)
                .DateTime("@InitialMovementDate", filter.InitialMovementDate?.Date)
                .DateTime("@FinalMovementDate", filter.FinalMovementDate?.Date)
                .Bit("@IsItemActive", filter.IsItemActive)
                .Bit("@IsReferenceActive", filter.IsReferenceActive)
                .Bit("@BombPrice", filter.BombPrice);
    }
}
