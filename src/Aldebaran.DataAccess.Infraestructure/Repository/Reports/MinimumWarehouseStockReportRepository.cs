using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class MinimumWarehouseStockReportRepository : RepositoryBase<AldebaranDbContext>, IMinimumWarehouseStockReportRepository
    {
        public MinimumWarehouseStockReportRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<MinimumWarehouseStockReport>> GetMinimumWarehouseStockReportDataAsync(MinimumWarehouseStockReportFilter filter, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(filter);
            var command = BuildCommand(filter);
            return await ExecuteQueryAsync(async dbContext =>
                await dbContext.Set<MinimumWarehouseStockReport>().FromSqlRaw(command.Sql, command.Parameters).ToListAsync(ct), ct);
        }

        /// <summary>Parámetros del SP (nombres originales del SP, incluidos sus errores de escritura).</summary>
        private static StoredProcedureCommand BuildCommand(MinimumWarehouseStockReportFilter filter) =>
            new StoredProcedureCommand("SP_GET_MINIMUM_WAREHOUSE_STOCK_REPORT")
                .Int("@MaximumQuantityTreshold", filter.MaximumQuantityThreshold)
                .Int("@WharehouseId", filter.WarehouseId > 0 ? filter.WarehouseId : null)
                .VarChar("@ItemList", filter.ItemIds)
                .VarChar("@ReferenceList", filter.ReferenceIds)
                .VarChar("@ProviderArticleName", filter.ProviderItemName, 50)
                .VarChar("@ProviderInternalReference", filter.ProviderReference, 50)
                .VarChar("@ProviderReferenceName", filter.ProviderReferenceName, 50)
                .VarChar("@ProviderReferenceCode", filter.ProviderReferenceCode, 50)
                .Bit("@ArticleActive", filter.ItemIsActive)
                .Bit("@DomesticProduct", filter.ItemIsDomesticProduct)
                .Bit("@SpetialImport", filter.ItemIsSpecialImport)
                .Bit("@Offert", filter.ItemIsSaleOff)
                .Bit("@CatalogVisible", filter.ItemIsCatalogVisible)
                .Bit("@BombPrice", filter.ItemIsBombPrice)
                .Bit("@ReferenceActive", filter.ReferenceIsActive)
                .Bit("@MinimumQuantityAlarm", filter.ReferenceHasAlarmMinimumQuantity)
                .Bit("@OutOfStockBIT", filter.ReferenceIsSoldOut);
    }
}
