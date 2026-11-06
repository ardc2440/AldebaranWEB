using Aldebaran.Infraestructure.Common.Utils;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Xunit;

namespace Aldebaran.Application.Services.Tests.Utils
{
    public class ImageThumbnailServiceTests : IDisposable
    {
        private readonly string _folder = Path.Combine(Path.GetTempPath(), "AldebaranThumbnailTests", Guid.NewGuid().ToString("N"));
        private readonly ImageThumbnailService _service = new();

        public ImageThumbnailServiceTests() => Directory.CreateDirectory(_folder);

        private string CreateImage(int width, int height)
        {
            var path = Path.Combine(_folder, $"{width}x{height}.jpg");
            using var image = new Image<Rgb24>(width, height);
            for (var y = 0; y < height; y++)
                for (var x = 0; x < width; x++)
                    image[x, y] = new Rgb24((byte)(x * 7 % 256), (byte)(y * 13 % 256), (byte)((x + y) % 256));
            image.SaveAsJpeg(path);
            return path;
        }

        [Fact]
        public void ImagenGrande_SeReduceAlTamanoMaximoYPesaMenos()
        {
            var source = CreateImage(1600, 1200);

            var thumbnail = _service.GetThumbnailPath(source, 200);

            Assert.NotEqual(source, thumbnail);
            using var image = Image.Load(thumbnail);
            Assert.Equal(200, image.Width);
            Assert.Equal(150, image.Height);
            Assert.True(new FileInfo(thumbnail).Length < new FileInfo(source).Length);
        }

        [Fact]
        public void MismaImagen_ReutilizaLaMiniatura()
        {
            var source = CreateImage(800, 800);

            Assert.Equal(_service.GetThumbnailPath(source, 200), _service.GetThumbnailPath(source, 200));
        }

        [Fact]
        public void ImagenIlegible_DevuelveLaRutaOriginal()
        {
            var source = Path.Combine(_folder, "danada.jpg");
            File.WriteAllText(source, "no es una imagen");

            Assert.Equal(source, _service.GetThumbnailPath(source, 200));
        }

        public void Dispose()
        {
            try { Directory.Delete(_folder, true); } catch { /* limpieza best-effort */ }
        }
    }
}
