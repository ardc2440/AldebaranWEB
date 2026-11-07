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
    }
}
