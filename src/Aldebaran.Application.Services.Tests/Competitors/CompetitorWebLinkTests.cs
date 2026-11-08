using Aldebaran.Application.Services.Competitors;
using Aldebaran.Application.Services.Models.Competitors;
using Xunit;

namespace Aldebaran.Application.Services.Tests.Competitors
{
    /// <summary>Direcciones web de la competencia: solo http/https llegan a un href.</summary>
    public class CompetitorWebLinkTests
    {
        [Theory]
        [InlineData("www.competidor.com", "https://www.competidor.com")]
        [InlineData("  competidor.com.co/catalogo  ", "https://competidor.com.co/catalogo")]
        [InlineData("http://competidor.com", "http://competidor.com")]
        [InlineData("https://competidor.com/p?id=1", "https://competidor.com/p?id=1")]
        public void DireccionValida_EnlaceSeguro(string value, string expected) =>
            Assert.Equal(expected, CompetitorWebLink.ToSafeHref(value));

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("javascript:alert(1)")]
        [InlineData("javascript://%0aalert(1)")]
        [InlineData("data://text/html,<script>alert(1)</script>")]
        [InlineData("ftp://competidor.com")]
        [InlineData("file://c:/windows")]
        [InlineData("competidor")]
        [InlineData("\"><script>alert(1)</script>")]
        public void DireccionNoValida_SinEnlace(string? value) => Assert.Null(CompetitorWebLink.ToSafeHref(value));

        [Theory]
        [InlineData("javascript://%0aalert(1)")]
        [InlineData("competidor")]
        public void Compania_SitioWebNoValido_Error(string website) =>
            Assert.Contains(CompetitorCompanyRules.Validate(CompetitorCompanyRules.Normalize(new CompetitorCompanyInput { CompanyName = "A", Website = website })),
                e => e.Contains("no es una dirección web válida"));

        [Fact]
        public void Compania_SitioWebSinEsquema_Valido() =>
            Assert.Empty(CompetitorCompanyRules.Validate(CompetitorCompanyRules.Normalize(new CompetitorCompanyInput { CompanyName = "A", Website = "nueva.com" })));

        [Fact]
        public void Homologacion_EnlaceJavascript_Error() =>
            Assert.Contains(CompetitorHomologationRules.Validate(CompetitorHomologationRules.Normalize(new CompetitorHomologationInput
            {
                CompetitorCompanyId = 1, ItemId = 1, CompetitorReference = "R", CompetitorProductName = "P", ProductUrl = "javascript://%0aalert(1)"
            })), e => e.Contains("enlace"));
    }
}
