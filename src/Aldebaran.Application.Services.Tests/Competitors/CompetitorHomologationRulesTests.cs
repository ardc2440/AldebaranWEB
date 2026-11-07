using Aldebaran.Application.Services.Competitors;
using Aldebaran.Application.Services.Models.Competitors;
using Xunit;

namespace Aldebaran.Application.Services.Tests.Competitors
{
    public class CompetitorHomologationRulesTests
    {
        private static CompetitorHomologationInput Valid() => new()
        {
            CompetitorCompanyId = 1, ItemId = 10, CompetitorReference = "CMP-1", CompetitorProductName = "Bolígrafo"
        };

        private static IReadOnlyList<string> Errors(CompetitorHomologationInput input) =>
            CompetitorHomologationRules.Validate(CompetitorHomologationRules.Normalize(input));

        [Fact]
        public void Minima_ANivelArticulo_EsValida()
        {
            var data = CompetitorHomologationRules.Normalize(Valid());
            Assert.Empty(CompetitorHomologationRules.Validate(data));
            Assert.Null(data.ReferenceId);
        }

        [Fact]
        public void SinCompaniaArticuloReferenciaNiNombre_CuatroErrores() =>
            Assert.Equal(4, Errors(new CompetitorHomologationInput()).Count);

        [Fact]
        public void Enlace_SinEsquema_SeAnteponeHttps()
        {
            var input = Valid(); input.ProductUrl = "  www.competidor.com/p/1 ";
            var data = CompetitorHomologationRules.Normalize(input);
            Assert.Equal("https://www.competidor.com/p/1", data.ProductUrl);
            Assert.Empty(CompetitorHomologationRules.Validate(data));
        }

        [Fact]
        public void Enlace_Vacio_QuedaNulo()
        {
            var input = Valid(); input.ProductUrl = "   ";
            Assert.Null(CompetitorHomologationRules.Normalize(input).ProductUrl);
        }

        [Theory]
        [InlineData("texto sin dominio")]
        [InlineData("ftp://competidor.com/p")]
        public void Enlace_Invalido_Error(string url)
        {
            var input = Valid(); input.ProductUrl = url;
            Assert.Single(Errors(input));
        }

        [Fact]
        public void Precio_Negativo_Error()
        {
            var input = Valid(); input.CompetitorPrice = -1;
            Assert.Single(Errors(input));
        }

        [Fact]
        public void Precio_Cero_EsValido()
        {
            var input = Valid(); input.CompetitorPrice = 0;
            Assert.Empty(Errors(input));
        }

        [Fact]
        public void ReferenciaCero_SeTrataComoNivelArticulo()
        {
            var input = Valid(); input.ReferenceId = 0;
            Assert.Null(CompetitorHomologationRules.Normalize(input).ReferenceId);
        }

        [Fact]
        public void Descripcion_YNivel()
        {
            var a = new CompetitorHomologation(1, 1, "C", true, "Importados", 10, "VA-1", "Agenda", null, null, null, "X", "Y", null, null, null, true, DateTime.Today);
            var r = a with { ReferenceId = 5, ReferenceCode = "01", ReferenceName = "Roja" };
            Assert.Equal("Artículo", a.Level);
            Assert.Equal("[VA-1] Agenda", a.PromosDescription);
            Assert.Equal("Referencia", r.Level);
            Assert.Equal("[VA-1] Agenda - Roja", r.PromosDescription);
        }
    }
}
