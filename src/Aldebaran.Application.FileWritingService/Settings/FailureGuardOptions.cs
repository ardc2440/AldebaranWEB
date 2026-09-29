namespace Aldebaran.Application.FileWritingService.Settings
{
    /// <summary>
    /// Parámetros de la sección "FailureGuard" del appsettings.json.
    /// </summary>
    public class FailureGuardOptions
    {
        public const string SectionName = "FailureGuard";

        /// <summary>
        /// Cada cuántos fallos consecutivos de generación se registra un evento crítico y se compacta
        /// la memoria del proceso. El servicio nunca se detiene por este motivo.
        /// </summary>
        public int MaxConsecutiveGenerationFailures { get; set; } = 3;
    }
}
