using Aldebaran.Application.Services.Competitors;
using Aldebaran.Web.Models;
using Microsoft.Extensions.Options;

namespace Aldebaran.Web.Services.Competitors
{
    /// <summary>
    /// Guarda las imágenes de productos de la competencia en la subcarpeta "Competencia" de AppSettings:ImageRepositoryPath,
    /// publicada por el sitio en /externalimages/Competencia/. Nombre: H{id}_{marca de tiempo}{ext} (cambia al reemplazar,
    /// así el navegador no muestra la imagen anterior en caché).
    /// </summary>
    public sealed class CompetitorImageStorage : ICompetitorImageStorage
    {
        public const string FolderName = "Competencia";
        /// <summary>Prefijo de la URL pública de las imágenes.</summary>
        public const string UrlPrefix = "/externalimages/" + FolderName + "/";
        private readonly string _folder;

        public CompetitorImageStorage(IOptions<AppSettings> settings)
        {
            var root = settings?.Value?.ImageRepositoryPath ?? throw new ArgumentNullException(nameof(settings));
            _folder = Path.Combine(Directory.GetCurrentDirectory(), root, FolderName);
        }

        /// <summary>URL pública de la imagen (null si no hay).</summary>
        public static string Url(string fileName) =>
            string.IsNullOrWhiteSpace(fileName) ? null : UrlPrefix + Uri.EscapeDataString(fileName);

        public async Task<string> SaveAsync(int competitorHomologationId, Stream content, string extension, CancellationToken ct = default)
        {
            Directory.CreateDirectory(_folder);
            var fileName = $"H{competitorHomologationId}_{DateTime.Now:yyyyMMddHHmmssfff}{extension}";
            await using var file = new FileStream(Path.Combine(_folder, fileName), FileMode.CreateNew, FileAccess.Write, FileShare.None);
            await content.CopyToAsync(file, ct);
            return fileName;
        }

        public async Task<byte[]> ReadAsync(string fileName, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return null;
            // Solo el nombre: nunca rutas recibidas desde fuera.
            var path = Path.Combine(_folder, Path.GetFileName(fileName));
            return File.Exists(path) ? await File.ReadAllBytesAsync(path, ct) : null;
        }

        public void Delete(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return;
            // Solo el nombre: nunca rutas recibidas desde fuera.
            var path = Path.Combine(_folder, Path.GetFileName(fileName));
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}
