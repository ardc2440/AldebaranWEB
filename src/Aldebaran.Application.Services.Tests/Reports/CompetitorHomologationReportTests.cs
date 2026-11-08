using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services.Reports;
using Xunit;
using Entities = Aldebaran.DataAccess.Entities.Reports;

namespace Aldebaran.Application.Services.Tests.Reports
{
    public class CompetitorHomologationReportTests
    {
        private static Entities.CompetitorHomologationReport Row(int company, string companyName, int item, string itemName, int? reference, string? referenceName, string competitorReference, int id) => new()
        {
            CompetitorCompanyId = company, CompanyName = companyName, Website = "w", LineId = 1, LineName = "Importados",
            ItemId = item, InternalReference = $"IR-{item}", ItemName = itemName,
            ReferenceId = reference, ReferenceCode = reference == null ? null : "01", ReferenceName = referenceName,
            CompetitorHomologationId = id, CompetitorReference = competitorReference, CompetitorProductName = $"P{id}", CompetitorPrice = 1000m * id
        };

        private static List<Entities.CompetitorHomologationReport> Rows() => new()
        {
            Row(2, "Zeta", 10, "Bolígrafo", 100, "Roja", "Z-2", 1),
            Row(1, "Alfa", 20, "Agenda", 200, "Negra", "A-2", 2),
            Row(1, "Alfa", 20, "Agenda", null, null, "A-1", 3),
            Row(1, "Alfa", 10, "Bolígrafo", null, null, "A-3", 4)
        };

        [Fact]
        public void Arbol_Vacio() => Assert.False(CompetitorHomologationReportTreeBuilder.Build(null).HasData);

        [Fact]
        public void Arbol_OrdenaYAgrupa_NivelArticuloPrimero()
        {
            var result = CompetitorHomologationReportTreeBuilder.Build(Rows());

            Assert.Equal(new[] { "Alfa", "Zeta" }, result.Companies.Select(c => c.CompanyName));
            Assert.Equal(4, result.HomologationCount);
            var alfaItems = Assert.Single(result.Companies[0].Lines).Items;
            Assert.Equal(new[] { "Agenda", "Bolígrafo" }, alfaItems.Select(i => i.ItemName));
            Assert.Equal(new[] { "Todo el artículo", "01 - Negra" }, alfaItems[0].Homologations.Select(h => h.PromosReference));
        }

        [Fact]
        public void Exportacion_UnaFilaPorHomologacion_MismoOrden()
        {
            var rows = CompetitorHomologationReportExportRowBuilder.Build(Rows());

            Assert.Equal(4, rows.Count);
            Assert.Equal(new[] { "A-1", "A-2", "A-3", "Z-2" }, rows.Select(r => r.CompetitorReference));
            Assert.Equal("Todo el artículo", rows[0].PromosReference);
            Assert.Equal(3000m, rows[0].CompetitorPrice);
        }

        [Fact]
        public void AlcanceHomologado_ArticuloCompletoOReferenciaPropia()
        {
            var catalog = new List<ItemReference>
            {
                new() { ItemId = 10, ReferenceId = 100 }, new() { ItemId = 10, ReferenceId = 101 },
                new() { ItemId = 20, ReferenceId = 200 }, new() { ItemId = 20, ReferenceId = 201 },
                new() { ItemId = 30, ReferenceId = 300 }
            };

            var scope = HomologatedScope.Apply(catalog, itemIds: new[] { 10 }, referenceIds: new[] { 201 });

            Assert.Equal(new[] { 100, 101, 201 }, scope.Select(r => r.ReferenceId));
        }
    }
}
