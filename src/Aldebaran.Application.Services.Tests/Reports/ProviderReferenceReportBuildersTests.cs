using Aldebaran.Application.Services.Reports;
using Xunit;
using Entities = Aldebaran.DataAccess.Entities.Reports;

namespace Aldebaran.Application.Services.Tests.Reports
{
    public class ProviderReferenceReportBuildersTests
    {
        private static Entities.ProviderReferenceReport Row(int provider, string providerName, short line, string lineName, int item, string itemName,
            int reference, string referenceName, short? warehouse = null, string? warehouseName = null, int? amount = null) => new()
        {
            ProviderId = provider, ProviderCode = $"P{provider}", ProviderName = providerName,
            LineId = line, LineCode = $"L{line}", LineName = lineName,
            ItemId = item, InternalReference = $"IR-{item}", ItemName = itemName,
            ReferenceId = reference, ReferenceCode = $"R{reference}", ReferenceName = referenceName, ProviderReferenceName = $"PR{reference}",
            ConfirmedAmount = 10, ReservedAmount = 2, AvailableAmount = 8,
            WarehouseId = warehouse, WarehouseName = warehouseName, Amount = amount
        };

        private static List<Entities.ProviderReferenceReport> WithInventory() => new()
        {
            Row(2, "Zeta", 1, "Importados", 10, "Bolígrafo", 100, "Azul", 2, "Zona Franca", 3),
            Row(2, "Zeta", 1, "Importados", 10, "Bolígrafo", 100, "Azul", 1, "Bodega Local", 5),
            Row(1, "Alfa", 1, "Importados", 20, "Agenda", 200, "Roja", 1, "Bodega Local", 7),
            Row(1, "Alfa", 1, "Importados", 20, "Agenda", 200, "Roja", 2, "Zona Franca", 0),
            Row(1, "Alfa", 1, "Importados", 20, "Agenda", 201, "Negra", 1, "Bodega Local", 1),
            Row(1, "Alfa", 1, "Importados", 20, "Agenda", 201, "Negra", 2, "Zona Franca", 4),
            // La misma referencia con otro proveedor no debe mezclar sus bodegas
            Row(2, "Zeta", 1, "Importados", 20, "Agenda", 200, "Roja", 1, "Bodega Local", 7),
            Row(2, "Zeta", 1, "Importados", 20, "Agenda", 200, "Roja", 2, "Zona Franca", 0)
        };

        [Fact]
        public void Arbol_SinFilas_EsVacio()
        {
            var result = ProviderReferenceReportTreeBuilder.Build(null, true);
            Assert.False(result.HasData);
            Assert.True(result.IncludeInventory);
        }

        [Fact]
        public void Arbol_OrdenaProveedoresYAgrupaSinDuplicar()
        {
            var result = ProviderReferenceReportTreeBuilder.Build(WithInventory(), true);

            Assert.Equal(new[] { "Alfa", "Zeta" }, result.Providers.Select(p => p.ProviderName));
            var alfa = result.Providers[0];
            var agenda = Assert.Single(Assert.Single(alfa.Lines).Items);
            Assert.Equal(new[] { "Negra", "Roja" }, agenda.References.Select(r => r.ReferenceName));

            var zeta = result.Providers[1];
            Assert.Equal(new[] { "Agenda", "Bolígrafo" }, Assert.Single(zeta.Lines).Items.Select(i => i.ItemName));
        }

        [Fact]
        public void Arbol_ConInventario_BodegasPorProveedorYReferencia()
        {
            var result = ProviderReferenceReportTreeBuilder.Build(WithInventory(), true);

            var roja = result.Providers[0].Lines[0].Items[0].References.Single(r => r.ReferenceId == 200);
            Assert.Equal(new[] { "Bodega Local", "Zona Franca" }, roja.Warehouses.Select(w => w.WarehouseName));
            Assert.Equal(new[] { 7, 0 }, roja.Warehouses.Select(w => w.Amount));

            var azul = result.Providers[1].Lines[0].Items.Single(i => i.ItemId == 10).References.Single();
            Assert.Equal(new[] { 5, 3 }, azul.Warehouses.Select(w => w.Amount));
        }

        [Fact]
        public void Arbol_SinInventario_SinBodegas()
        {
            var rows = WithInventory().DistinctBy(r => (r.ProviderId, r.ReferenceId))
                                      .Select(r => { r.WarehouseId = null; r.WarehouseName = null; r.Amount = null; return r; });

            var result = ProviderReferenceReportTreeBuilder.Build(rows, false);

            Assert.False(result.IncludeInventory);
            Assert.All(result.Providers.SelectMany(p => p.Lines).SelectMany(l => l.Items).SelectMany(i => i.References),
                       r => Assert.Empty(r.Warehouses));
            Assert.Equal(4, result.Providers.SelectMany(p => p.Lines).SelectMany(l => l.Items).SelectMany(i => i.References).Count());
        }

        [Fact]
        public void Exportacion_ConInventario_UnaFilaPorProveedorReferencia_ConBodegas()
        {
            var export = ProviderReferenceReportExportRowBuilder.Build(WithInventory(), true);

            Assert.True(export.IncludeInventory);
            Assert.Empty(export.References);
            Assert.Equal(4, export.Count);
            var rows = export.InventoryReferences;
            Assert.Equal(new[] { "Alfa", "Alfa", "Zeta", "Zeta" }, rows.Select(r => r.ProviderName));
            var negra = rows[0];
            Assert.Equal("Negra", negra.ReferenceName);
            Assert.Equal(8, negra.AvailableAmount);
            Assert.Equal(1, negra.LocalWarehouse);
            Assert.Equal(4, negra.FreeZone);
            var azul = rows.Single(r => r.ReferenceName == "Azul");
            Assert.Equal(5, azul.LocalWarehouse);
            Assert.Equal(3, azul.FreeZone);
        }

        [Fact]
        public void Exportacion_SinInventario_SoloColumnasDeReferencia()
        {
            var rows = WithInventory().DistinctBy(r => (r.ProviderId, r.ReferenceId))
                                      .Select(r => { r.WarehouseId = null; r.WarehouseName = null; r.Amount = null; return r; });

            var export = ProviderReferenceReportExportRowBuilder.Build(rows, false);

            Assert.False(export.IncludeInventory);
            Assert.Empty(export.InventoryReferences);
            Assert.Equal(4, export.Count);
            Assert.Equal("PR201", export.References[0].ProviderReferenceName);
        }

        [Fact]
        public void Exportacion_ColumnasSinInventario_NoIncluyenCantidades()
        {
            var columns = typeof(Aldebaran.Application.Services.Models.Reports.ProviderReferenceReportExportRow).GetProperties().Select(p => p.Name).ToList();
            Assert.DoesNotContain("AvailableAmount", columns);
            Assert.DoesNotContain("LocalWarehouse", columns);
        }
    }
}
