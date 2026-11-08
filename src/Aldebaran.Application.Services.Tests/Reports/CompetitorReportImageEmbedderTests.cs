using Aldebaran.Application.Services.Reports;
using Xunit;

namespace Aldebaran.Application.Services.Tests.Reports
{
    /// <summary>Incrustación de las imágenes de la competencia en el HTML del PDF del reporte de homologaciones.</summary>
    public class CompetitorReportImageEmbedderTests
    {
        private const string Prefix = "/externalimages/Competencia/";
        private static readonly byte[] Png = { 137, 80, 78, 71 };

        private static Func<string, Task<byte[]?>> Reader(Dictionary<string, byte[]> files, List<string>? reads = null) => name =>
        {
            reads?.Add(name);
            return Task.FromResult<byte[]?>(files.TryGetValue(name, out var b) ? b : null);
        };

        [Fact]
        public async Task ImagenExistente_SeIncrustaComoDataUri_ConservaAtributos()
        {
            var html = "<td><img src=\"/externalimages/Competencia/H1_1.png\" alt=\"Mug\" style=\"max-width:70px\"></td>";
            var result = await CompetitorReportImageEmbedder.EmbedAsync(html, Prefix, Reader(new() { ["H1_1.png"] = Png }));
            Assert.Equal($"<td><img src=\"data:image/png;base64,{Convert.ToBase64String(Png)}\" alt=\"Mug\" style=\"max-width:70px\"></td>", result);
        }

        [Fact]
        public async Task ImagenInexistente_SeQuitaLaEtiqueta()
        {
            var result = await CompetitorReportImageEmbedder.EmbedAsync("<p>a<img src=\"/externalimages/Competencia/H9_1.jpg\">b</p>", Prefix, Reader(new()));
            Assert.Equal("<p>ab</p>", result);
        }

        [Fact]
        public async Task OtrasImagenes_NoSeTocan()
        {
            var html = "<img src=\"/images/logo.png\"><img src=\"/externalimages/Otra/H1_1.png\">";
            var reads = new List<string>();
            Assert.Equal(html, await CompetitorReportImageEmbedder.EmbedAsync(html, Prefix, Reader(new(), reads)));
            Assert.Empty(reads);
        }

        [Fact]
        public async Task NombreEscapado_SeLeeDesescapado_YRepetidas_UnaSolaLectura()
        {
            var html = "<img src=\"/externalimages/Competencia/H1%20a.jpeg\"><img src=\"/externalimages/Competencia/H1%20a.jpeg\">";
            var reads = new List<string>();
            var result = await CompetitorReportImageEmbedder.EmbedAsync(html, Prefix, Reader(new() { ["H1 a.jpeg"] = Png }, reads));
            Assert.Equal(new[] { "H1 a.jpeg" }, reads);
            Assert.Equal(2, result.Split("data:image/jpeg;base64,").Length - 1);
        }

        [Fact]
        public async Task HtmlVacio_SinCambios()
        {
            Assert.Equal("", await CompetitorReportImageEmbedder.EmbedAsync("", Prefix, Reader(new())));
        }

        [Theory]
        [InlineData("a.jpg", "image/jpeg")]
        [InlineData("a.JPEG", "image/jpeg")]
        [InlineData("a.png", "image/png")]
        [InlineData("a.webp", "image/webp")]
        public void DataUri_TipoPorExtension(string name, string mime) =>
            Assert.StartsWith($"data:{mime};base64,", CompetitorReportImageEmbedder.ToDataUri(name, Png));

        [Theory]
        [InlineData("a.gif")]
        [InlineData("a")]
        public void DataUri_ExtensionNoPermitida_Null(string name) => Assert.Null(CompetitorReportImageEmbedder.ToDataUri(name, Png));

        [Fact]
        public void DataUri_SinContenido_Null() => Assert.Null(CompetitorReportImageEmbedder.ToDataUri("a.png", Array.Empty<byte>()));
    }
}
