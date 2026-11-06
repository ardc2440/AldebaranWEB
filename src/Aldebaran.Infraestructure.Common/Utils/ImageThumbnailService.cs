using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System.Security.Cryptography;
using System.Text;

namespace Aldebaran.Infraestructure.Common.Utils
{
    public class ImageThumbnailService : IImageThumbnailService
    {
        private const int JpegQuality = 75;
        private static readonly string CacheFolder = Path.Combine(Path.GetTempPath(), "AldebaranThumbnails");

        public string GetThumbnailPath(string sourcePath, int maxSize)
        {
            try
            {
                var thumbnailPath = BuildCachePath(sourcePath, maxSize);

                if (!File.Exists(thumbnailPath))
                    CreateThumbnail(sourcePath, thumbnailPath, maxSize);

                return thumbnailPath;
            }
            catch
            {
                /* Imagen ilegible o sin permisos en la carpeta temporal: se usa la original
                   (el control de tamaño del correo cubre el caso) */
                return sourcePath;
            }
        }

        /// <summary>Nombre único por ruta, fecha de modificación y tamaño: si la imagen cambia, se genera otra miniatura.</summary>
        private static string BuildCachePath(string sourcePath, int maxSize)
        {
            var info = new FileInfo(sourcePath);
            var key = $"{info.FullName.ToUpperInvariant()}|{info.LastWriteTimeUtc.Ticks}|{info.Length}|{maxSize}";
            var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(key)))[..32];

            return Path.Combine(CacheFolder, $"{hash}.jpg");
        }

        private static void CreateThumbnail(string sourcePath, string thumbnailPath, int maxSize)
        {
            Directory.CreateDirectory(CacheFolder);

            using var image = Image.Load(sourcePath);

            if (image.Width > maxSize || image.Height > maxSize)
                image.Mutate(x => x.Resize(new ResizeOptions { Mode = ResizeMode.Max, Size = new Size(maxSize, maxSize) }));

            /* Archivo temporal + Move: otro proceso nunca lee una miniatura a medio escribir */
            var tempPath = $"{thumbnailPath}.{Guid.NewGuid():N}.tmp";
            image.SaveAsJpeg(tempPath, new JpegEncoder { Quality = JpegQuality });
            File.Move(tempPath, thumbnailPath, overwrite: true);
        }
    }
}
