using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

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
                var parameters = new[]
                {
                    IdsParameter("@LineIds", filter.LineIds),
                    IdsParameter("@ItemIds", filter.ItemIds),
                    IdsParameter("@ReferenceIds", filter.ReferenceIds),
                    BitParameter("@OnlyActiveItems", filter.OnlyActiveItems),
                    BitParameter("@OnlyCatalogVisible", filter.OnlyCatalogVisible),
                    BitParameter("@OnlyDomesticProduct", filter.OnlyDomesticProduct),
                    BitParameter("@OnlySpecialImport", filter.OnlySpecialImport),
                    BitParameter("@OnlySaleOff", filter.OnlySaleOff),
                    BitParameter("@OnlyActiveReferences", filter.OnlyActiveReferences),
                    BitParameter("@OnlySoldOut", filter.OnlySoldOut),
                    BitParameter("@OnlyWithAlarmMinimumQuantity", filter.OnlyWithAlarmMinimumQuantity),
                    BitParameter("@OnlyWithMinimumLocalWarehouseQuantity", filter.OnlyWithMinimumLocalWarehouseQuantity)
                };

                return await dbContext.Set<ItemReferencesReport>()
                    .FromSqlRaw("EXEC dbo.SP_GET_ITEM_REFERENCES_REPORT " +
                                "@LineIds, @ItemIds, @ReferenceIds, " +
                                "@OnlyActiveItems, @OnlyCatalogVisible, @OnlyDomesticProduct, @OnlySpecialImport, @OnlySaleOff, " +
                                "@OnlyActiveReferences, @OnlySoldOut, @OnlyWithAlarmMinimumQuantity, @OnlyWithMinimumLocalWarehouseQuantity",
                                parameters)
                    .AsNoTracking()
                    .ToListAsync(ct);
            }, ct);
        }

        /// <summary>Lista de Ids separada por coma; sin Ids se envía NULL (el SP lo interpreta como "sin filtro").</summary>
        private static SqlParameter IdsParameter<T>(string name, IEnumerable<T>? ids) =>
            new(name, SqlDbType.VarChar, -1)
            {
                Value = ids?.Any() == true ? string.Join(",", ids.Distinct()) : DBNull.Value
            };

        private static SqlParameter BitParameter(string name, bool value) =>
            new(name, SqlDbType.Bit) { Value = value };
    }
}
