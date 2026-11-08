using Aldebaran.Application.Services.Competitors;
using Aldebaran.Application.Services.Models.Competitors;
using Xunit;

namespace Aldebaran.Application.Services.Tests.Competitors
{
    public class CompetitorCompanyRulesTests
    {
        private static IReadOnlyList<string> Errors(CompetitorCompanyInput input) =>
            CompetitorCompanyRules.Validate(CompetitorCompanyRules.Normalize(input));

        [Fact]
        public void Minima_NombreYSitioWeb_EsValida() =>
            Assert.Empty(Errors(new CompetitorCompanyInput { CompanyName = "Competidor", Website = "www.c.com" }));

        [Fact]
        public void SinNombreNiSitioWeb_DosErrores() =>
            Assert.Equal(2, Errors(new CompetitorCompanyInput { CompanyName = "  ", Website = "" }).Count);

        [Fact]
        public void Normaliza_RecortaYOpcionalesVaciosQuedanNulos()
        {
            var data = CompetitorCompanyRules.Normalize(new CompetitorCompanyInput { CompanyName = "  A  ", Website = " w ", ContactPerson = "   ", Phone = "" });
            Assert.Equal("A", data.CompanyName);
            Assert.Equal("w", data.Website);
            Assert.Null(data.ContactPerson);
            Assert.Null(data.Phone);
        }

        [Fact]
        public void Normaliza_SinPais_DescartaDepartamentoYCiudad()
        {
            var data = CompetitorCompanyRules.Normalize(new CompetitorCompanyInput { CompanyName = "A", Website = "w", DepartmentId = 5, CityId = 9 });
            Assert.Null(data.DepartmentId);
            Assert.Null(data.CityId);
        }

        [Fact]
        public void Normaliza_SinDepartamento_DescartaCiudad()
        {
            var data = CompetitorCompanyRules.Normalize(new CompetitorCompanyInput { CompanyName = "A", Website = "w", CountryId = 1, CityId = 9 });
            Assert.Equal(1, data.CountryId);
            Assert.Null(data.CityId);
        }

        [Fact]
        public void Longitudes_SuperanColumnas_Errores()
        {
            var errors = Errors(new CompetitorCompanyInput
            {
                CompanyName = new string('a', CompetitorCompanyRules.CompanyNameMaxLength + 1),
                Website = "www.c.com",
                Phone = new string('1', CompetitorCompanyRules.PhoneMaxLength + 1)
            });
            Assert.Equal(2, errors.Count);
        }

        [Fact]
        public void Ubicacion_TextoLegible() =>
            Assert.Equal("Bogotá, Cundinamarca, Colombia",
                new CompetitorCompany(1, "A", "w", null, null, 1, "Colombia", 2, "Cundinamarca", 3, "Bogotá", true, DateTime.Today).Location);
    }
}
