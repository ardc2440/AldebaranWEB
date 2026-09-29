namespace Aldebaran.Application.FileWritingService.Settings
{
    /// <summary>
    /// Parámetros de la sección "FtpResilience" del appsettings.json.
    /// </summary>
    public class FtpResilienceOptions
    {
        public const string SectionName = "FtpResilience";

        public int MaxParallelUploads { get; set; } = 4;
    }
}
