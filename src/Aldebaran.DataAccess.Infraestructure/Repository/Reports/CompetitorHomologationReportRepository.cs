using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class CompetitorHomologationReportRepository : RepositoryBase<AldebaranDbContext>, ICompetitorHomologationReportRepository
    {
        public CompetitorHomologationReportRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<CompetitorHomologationReport>> GetReportDataAsync(CompetitorHomologationReportFilter filter, CancellationToken ct = default)
        {
            filter ??= new CompetitorHomologationReportFilter();

            return await ExecuteQueryAsync(async dbContext =>
            {
                var command = new StoredProcedureCommand("dbo.SP_GET_COMPETITOR_HOMOLOGATION_REPORT")
                    .VarChar("@LineIds", Ids(filter.LineIds))
                    .VarChar("@ItemIds", Ids(filter.ItemIds))
                    .VarChar("@ReferenceIds", Ids(filter.ReferenceIds))
                    .VarChar("@CompetitorCompanyIds", Ids(filter.CompetitorCompanyIds))
                    .VarChar("@CompetitorReference", filter.CompetitorReference, 50)
                    .VarChar("@CompetitorProductName", filter.CompetitorProductName, 150)
                    .Bit("@IncludeInactive", filter.IncludeInactive);

                return await dbContext.Set<CompetitorHomologationReport>()
                    .FromSqlRaw(command.Sql, command.Parameters)
                    .AsNoTracking()
                    .ToListAsync(ct);
            }, ct);
        }

        public async Task<(IReadOnlyList<int> ItemIds, IReadOnlyList<int> ReferenceIds)> GetHomologatedScopeAsync(CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                var active = dbContext.CompetitorHomologations.AsNoTracking().Where(h => h.IsActive && h.CompetitorCompany.IsActive);
                var itemIds = await active.Where(h => h.ReferenceId == null).Select(h => h.ItemId).Distinct().ToListAsync(ct);
                var referenceIds = await active.Where(h => h.ReferenceId != null).Select(h => h.ReferenceId!.Value).Distinct().ToListAsync(ct);
                return ((IReadOnlyList<int>)itemIds, (IReadOnlyList<int>)referenceIds);
            }, ct);
        }

        /// <summary>Lista de Ids separada por coma; sin Ids no se envía (el SP lo interpreta como "sin filtro").</summary>
        private static string? Ids<T>(IEnumerable<T>? ids) => ids?.Any() == true ? string.Join(",", ids.Distinct()) : null;
    }
}
