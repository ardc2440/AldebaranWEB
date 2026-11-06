using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class ProviderReferenceReportRepository : RepositoryBase<AldebaranDbContext>, IProviderReferenceReportRepository
    {
        public ProviderReferenceReportRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<ProviderReferenceReport>> GetProviderReferenceReportDataAsync(ProviderReferenceReportFilter filter, CancellationToken ct = default)
        {
            filter ??= new ProviderReferenceReportFilter();

            return await ExecuteQueryAsync(async dbContext =>
            {
                var command = new StoredProcedureCommand("dbo.SP_GET_PROVIDER_REFERENCE_REPORT")
                    .Int("@ProviderId", filter.ProviderId)
                    .VarChar("@LineIds", Ids(filter.LineIds))
                    .VarChar("@ItemIds", Ids(filter.ItemIds))
                    .VarChar("@ReferenceIds", Ids(filter.ReferenceIds))
                    .Bit("@IsItemActive", filter.IsItemActive)
                    .Bit("@IsCatalogVisible", filter.IsCatalogVisible)
                    .Bit("@IsDomesticProduct", filter.IsDomesticProduct)
                    .Bit("@IsSpecialImport", filter.IsSpecialImport)
                    .Bit("@IsSaleOff", filter.IsSaleOff)
                    .Bit("@BombPrice", filter.BombPrice)
                    .Bit("@IsReferenceActive", filter.IsReferenceActive)
                    .Bit("@IsSoldOut", filter.IsSoldOut)
                    .Bit("@IncludeInventory", filter.IncludeInventory);

                return await dbContext.Set<ProviderReferenceReport>()
                    .FromSqlRaw(command.Sql, command.Parameters)
                    .AsNoTracking()
                    .ToListAsync(ct);
            }, ct);
        }

        [Obsolete("Filtro armado como texto. Se retira en T3 (la pantalla pasa a usar el filtro tipado).")]
        public async Task<IEnumerable<ProviderReferenceReport>> GetProviderReferenceReportDataAsync(string filter, CancellationToken ct)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Set<ProviderReferenceReport>().FromSqlRaw($"EXEC SP_GET_PROVIDER_REFERENCE_REPORT {filter}").ToListAsync(ct);
            }, ct);
        }

        /// <summary>Lista de Ids separada por coma; sin Ids no se envía (el SP lo interpreta como "sin filtro").</summary>
        private static string? Ids<T>(IEnumerable<T>? ids) => ids?.Any() == true ? string.Join(",", ids.Distinct()) : null;
    }
}
