using Microsoft.AspNetCore.Mvc;

namespace Aldebaran.Web.Controllers
{
    public class UploadController : Controller
    {
        private readonly IWebHostEnvironment environment;

        public UploadController(IWebHostEnvironment environment)
        {
            this.environment = environment;
        }

        [HttpPost("upload/image")]
        public IActionResult Image(IFormFile file)
        {
            try
            {
                // Verificar si el archivo es una imagen
                if (file == null || file.Length == 0 || !IsImage(file))
                {
                    return BadRequest("Archivo no válido. Por favor, seleccione una imagen.");
                }
                // Verificar el tamaño del archivo
                if (file.Length > 2 * 1024 * 1024) // 2MB en bytes
                {
                    return BadRequest("El tamaño máximo permitido para la imagen es de 2MB.");
                }

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var directory = Path.Combine(environment.WebRootPath, "shared");
                var filePath = Path.Combine(directory, fileName);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                    var url = GetRequestUrl(Request, filePath);
                    return Ok(new { Url = url });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        string GetRequestUrl(HttpRequest request, string filePath)
        {
            var baseUrl = $"{request.Scheme}://{request.Host}";
            var relativePath = filePath.Replace(environment.WebRootPath, "").Replace('\\', '/').TrimStart('/');
            return $"{baseUrl}/{relativePath}";
        }
        bool IsImage(IFormFile file)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return allowedExtensions.Contains(extension);
        }
    }
}
