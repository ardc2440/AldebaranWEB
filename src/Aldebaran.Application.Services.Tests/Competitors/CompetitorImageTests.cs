using Aldebaran.Application.Services.Competitors;
using Aldebaran.DataAccess.Infraestructure.Repository.Competitors;
using Xunit;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services.Tests.Competitors
{
    public class CompetitorImageTests
    {
        [Theory]
        [InlineData("foto.JPG", ".jpg")]
        [InlineData("foto.jpeg", ".jpeg")]
        [InlineData("foto.png", ".png")]
        [InlineData("foto.webp", ".webp")]
        [InlineData("foto.gif", null)]
        [InlineData("foto.exe", null)]
        [InlineData("sin-extension", null)]
        public void Extension_Permitida(string fileName, string? expected) =>
            Assert.Equal(expected, CompetitorImageRules.NormalizeExtension(fileName));

        [Fact]
        public void Tamano_SuperaMaximo_Error() =>
            Assert.NotNull(CompetitorImageRules.Validate("a.jpg", CompetitorImageRules.MaxBytes + 1));

        [Fact]
        public void Tamano_Vacio_Error() => Assert.NotNull(CompetitorImageRules.Validate("a.jpg", 0));

        [Fact]
        public void Valida() => Assert.Null(CompetitorImageRules.Validate("a.png", 1024));

        #region Servicio
        private static MemoryStream Jpg() => new(new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0, 0, 0, 0, 0, 0 });
        private static MemoryStream Png() => new(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0, 0 });

        [Fact]
        public async Task ContenidoQueNoEsImagen_NoGuarda()
        {
            var storage = new FakeStorage();
            var html = new MemoryStream(System.Text.Encoding.ASCII.GetBytes("<html><script>alert(1)</script></html>"));
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => new CompetitorHomologationService(new Repo(), storage).SetImageAsync(1, html, "falsa.jpg", html.Length));
            Assert.Equal(CompetitorImageRules.InvalidContentMessage, ex.Message);
            Assert.Empty(storage.Files);
        }

        [Theory]
        [InlineData(".jpg", new byte[] { 0xFF, 0xD8, 0xFF, 0xDB }, true)]
        [InlineData(".jpeg", new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 }, true)]
        [InlineData(".png", new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, true)]
        [InlineData(".webp", new byte[] { 0x52, 0x49, 0x46, 0x46, 1, 2, 3, 4, 0x57, 0x45, 0x42, 0x50 }, true)]
        [InlineData(".png", new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }, false)]
        [InlineData(".jpg", new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, false)]
        [InlineData(".jpg", new byte[] { 0x3C, 0x68, 0x74, 0x6D, 0x6C }, false)]
        [InlineData(".gif", new byte[] { 0x47, 0x49, 0x46, 0x38 }, false)]
        [InlineData(".jpg", new byte[0], false)]
        public void Firma_SegunExtension(string extension, byte[] header, bool expected) =>
            Assert.Equal(expected, CompetitorImageRules.MatchesSignature(header, extension));

        private sealed class FakeStorage : ICompetitorImageStorage
        {
            public List<string> Files { get; } = new();
            private int _n;
            public Task<string> SaveAsync(int id, Stream content, string extension, CancellationToken ct = default)
            {
                var name = $"H{id}_{++_n}{extension}"; Files.Add(name); return Task.FromResult(name);
            }
            public Task<byte[]?> ReadAsync(string? fileName, CancellationToken ct = default) =>
                Task.FromResult<byte[]?>(fileName != null && Files.Contains(fileName) ? new byte[] { 1 } : null);
            public void Delete(string? fileName) { if (fileName != null) Files.Remove(fileName); }
        }

        private sealed class Repo : ICompetitorHomologationRepository
        {
            public Entities.CompetitorHomologation H { get; } = new() { CompetitorHomologationId = 1, IsActive = true, CompetitorReference = "X", CompetitorProductName = "Y" };
            public bool FailOnSetImage { get; set; }
            public Task<Entities.CompetitorHomologation?> FindAsync(int id, CancellationToken ct = default) => Task.FromResult<Entities.CompetitorHomologation?>(id == 1 ? H : null);
            public Task SetImageFileNameAsync(int id, string? imageFileName, CancellationToken ct = default)
            {
                if (FailOnSetImage) throw new InvalidOperationException("fallo BD");
                H.ImageFileName = imageFileName; return Task.CompletedTask;
            }
            public Task<IReadOnlyList<Entities.CompetitorHomologation>> GetAsync(CompetitorHomologationQuery query, CancellationToken ct = default) => throw new NotImplementedException();
            public Task<bool> ExistsActiveAsync(int a, int b, int? c, string d, int? e, CancellationToken ct = default) => throw new NotImplementedException();
            public Task<bool> IsActiveCompanyAsync(int a, CancellationToken ct = default) => throw new NotImplementedException();
            public Task<bool> ItemExistsAsync(int a, CancellationToken ct = default) => throw new NotImplementedException();
            public Task<bool> ReferenceBelongsToItemAsync(int a, int b, CancellationToken ct = default) => throw new NotImplementedException();
            public Task<int> AddAsync(Entities.CompetitorHomologation h, CancellationToken ct = default) => throw new NotImplementedException();
            public Task UpdateAsync(int id, Entities.CompetitorHomologation h, CancellationToken ct = default) => throw new NotImplementedException();
            public Task SetActiveAsync(int id, bool isActive, CancellationToken ct = default) => throw new NotImplementedException();
        }

        [Fact]
        public async Task Reemplazar_EliminaLaAnterior()
        {
            var repo = new Repo(); var storage = new FakeStorage();
            var service = new CompetitorHomologationService(repo, storage);
            await service.SetImageAsync(1, Jpg(), "a.jpg", 10);
            var first = repo.H.ImageFileName;
            await service.SetImageAsync(1, Png(), "b.png", 10);

            Assert.NotEqual(first, repo.H.ImageFileName);
            Assert.Equal(new[] { repo.H.ImageFileName! }, storage.Files);
        }

        [Fact]
        public async Task FalloAlRegistrar_NoDejaArchivoHuerfano()
        {
            var repo = new Repo { FailOnSetImage = true }; var storage = new FakeStorage();
            await Assert.ThrowsAsync<InvalidOperationException>(() => new CompetitorHomologationService(repo, storage).SetImageAsync(1, Jpg(), "a.jpg", 10));
            Assert.Empty(storage.Files);
        }

        [Fact]
        public async Task Quitar_LimpiaRegistroYArchivo()
        {
            var repo = new Repo(); var storage = new FakeStorage();
            var service = new CompetitorHomologationService(repo, storage);
            await service.SetImageAsync(1, Jpg(), "a.jpg", 10);
            await service.RemoveImageAsync(1);
            Assert.Null(repo.H.ImageFileName);
            Assert.Empty(storage.Files);
        }

        [Fact]
        public async Task TipoNoPermitido_NoGuarda()
        {
            var storage = new FakeStorage();
            await Assert.ThrowsAsync<ArgumentException>(() => new CompetitorHomologationService(new Repo(), storage).SetImageAsync(1, new MemoryStream(new byte[10]), "a.gif", 10));
            Assert.Empty(storage.Files);
        }
        #endregion
    }
}
