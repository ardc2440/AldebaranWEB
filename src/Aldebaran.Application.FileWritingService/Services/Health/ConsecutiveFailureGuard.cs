using Microsoft.Extensions.Logging;
using System.Runtime;

namespace Aldebaran.Application.FileWritingService.Services.Health
{
    /// <summary>
    /// Cuenta fallos consecutivos de generación de un worker. Nunca detiene el servicio:
    /// al alcanzar el umbral registra un evento crítico y compacta la memoria del proceso
    /// (incluido el Large Object Heap) para que la siguiente ejecución tenga espacio.
    /// </summary>
    internal sealed class ConsecutiveFailureGuard
    {
        private readonly string _workerName;
        private readonly int _threshold;
        private readonly ILogger _logger;
        private int _consecutiveFailures;

        public ConsecutiveFailureGuard(string workerName, int threshold, ILogger logger)
        {
            _workerName = workerName;
            _threshold = Math.Max(1, threshold);
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void RegisterSuccess()
        {
            if (_consecutiveFailures > 0)
                _logger.LogInformation("{Worker}: generación recuperada después de {Failures} fallos consecutivos", _workerName, _consecutiveFailures);

            _consecutiveFailures = 0;
        }

        public void RegisterFailure()
        {
            _consecutiveFailures++;
            _logger.LogWarning("{Worker}: fallo de generación consecutivo número {Failures}", _workerName, _consecutiveFailures);

            if (_consecutiveFailures % _threshold == 0)
                CompactMemory();
        }

        private void CompactMemory()
        {
            _logger.LogCritical("{Worker}: {Failures} fallos consecutivos de generación. Se compacta la memoria del proceso; el servicio sigue en ejecución.", _workerName, _consecutiveFailures);
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true, compacting: true);
            GC.WaitForPendingFinalizers();
        }
    }
}
