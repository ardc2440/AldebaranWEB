using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class ItemReferencesReportRepository : RepositoryBase<AldebaranDbContext>, IItemReferencesReportRepository
    {
        public ItemReferencesReportRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<ItemReferencesReport>> GetItemReferencesReportDataAsync(ItemReferencesReportFilter filter, CancellationToken ct = default)
        {
            filter ??= new ItemReferencesReportFilter();

            return await ExecuteQueryAsync(async dbContext =>
            {
                var command = new StoredProcedureCommand("dbo.SP_GET_ITEM_REFERENCES_REPORT")
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
                    .Bit("@HasAlarmMinimumQuantity", filter.HasAlarmMinimumQuantity)
                    .Bit("@HasMinimumLocalWarehouseQuantity", filter.HasMinimumLocalWarehouseQuantity);

                return await dbContext.Set<ItemReferencesReport>()
                    .FromSqlRaw(command.Sql, command.Parameters)
                    .AsNoTracking()
                    .ToListAsync(ct);
            }, ct);
        }

        /// <summary>Lista de Ids separada por coma; sin Ids no se envía (el SP lo interpreta como "sin filtro").</summary>
        private static string? Ids<T>(IEnumerable<T>? ids) => ids?.Any() == true ? string.Join(",", ids.Distinct()) : null;
    }
}
