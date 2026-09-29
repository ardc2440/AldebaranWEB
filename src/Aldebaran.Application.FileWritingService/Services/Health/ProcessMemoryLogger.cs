using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Aldebaran.Application.FileWritingService.Services.Health
{
    /// <summary>
    /// Registra el consumo de memoria del proceso para poder ver su tendencia en los logs.
    /// </summary>
    internal static class ProcessMemoryLogger
    {
        private const double BytesPerMegabyte = 1024d * 1024d;

        public static void Log(ILogger logger, string workerName)
        {
            using var process = Process.GetCurrentProcess();
            var workingSetMb = process.WorkingSet64 / BytesPerMegabyte;
            var privateMb = process.PrivateMemorySize64 / BytesPerMegabyte;
            var gcHeapMb = GC.GetTotalMemory(forceFullCollection: false) / BytesPerMegabyte;

            logger.LogInformation("{Worker} memoria: WorkingSet={WorkingSetMb:F0} MB, Private={PrivateMb:F0} MB, GcHeap={GcHeapMb:F0} MB, Proceso64Bits={Is64Bit}",
                workerName, workingSetMb, privateMb, gcHeapMb, Environment.Is64BitProcess);
        }
    }
}
