namespace Aldebaran.Application.Services.Competitors
{
    /// <summary>Reglas de la imagen del producto de la competencia: tipos permitidos y tamaño máximo.</summary>
    public static class CompetitorImageRules
    {
        public const long MaxBytes = 2 * 1024 * 1024;
        public static readonly IReadOnlyList<string> AllowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        public static string AcceptAttribute => string.Join(",", AllowedExtensions);

        /// <summary>Extensión normalizada (minúsculas) o null si no es un tipo permitido.</summary>
        public static string? NormalizeExtension(string? fileName)
        {
            var extension = Path.GetExtension(fileName ?? string.Empty).ToLowerInvariant();
            return AllowedExtensions.Contains(extension) ? extension : null;
        }

        /// <summary>Error para el usuario o null si la imagen es válida.</summary>
        public static string? Validate(string? fileName, long size)
        {
            if (NormalizeExtension(fileName) == null)
                return "La imagen debe ser JPG, PNG o WEBP.";
            if (size <= 0)
                return "El archivo de imagen está vacío.";
            if (size > MaxBytes)
                return $"La imagen supera el tamaño máximo de {MaxBytes / (1024 * 1024)} MB.";
            return null;
        }

        /// <summary>Mensaje cuando el contenido no corresponde a una imagen del tipo indicado por la extensión.</summary>
        public const string InvalidContentMessage = "El archivo no es una imagen JPG, PNG o WEBP válida.";

        /// <summary>
        /// El contenido corresponde al tipo de la extensión (firma del archivo): evita que un archivo renombrado
        /// (por ejemplo HTML o un ejecutable con extensión .jpg) quede publicado como imagen.
        /// </summary>
        public static bool MatchesSignature(ReadOnlySpan<byte> header, string? extension) => extension switch
        {
            ".jpg" or ".jpeg" => header.Length >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF,
            ".png" => header.Length >= 8 && header[..8].SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }),
            ".webp" => header.Length >= 12 && header[..4].SequenceEqual("RIFF"u8) && header[8..12].SequenceEqual("WEBP"u8),
            _ => false
        };
    }
}
