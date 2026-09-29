namespace Aldebaran.Application.FileWritingService.Settings
{
    /// <summary>
    /// Parámetros de la sección "PdfGeneration" del appsettings.json.
    /// </summary>
    public class PdfGenerationOptions
    {
        public const string SectionName = "PdfGeneration";

        /// <summary>Tiempo máximo (segundos) para que Chromium cargue el HTML del reporte.</summary>
        public int LoadTimeoutSeconds { get; set; } = 120;

        /// <summary>Tiempo máximo (segundos) para que Chromium genere el archivo PDF.</summary>
        public int PdfTimeoutSeconds { get; set; } = 300;
    }
}
