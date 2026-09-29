using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Aldebaran.Application.FileWritingService.Resilience
{
    /// <summary>
    /// Ejecuta una operación con reintentos (backoff exponencial + jitter) y un tiempo máximo por intento.
    /// - El timeout se aplica cancelando el token del intento: no quedan operaciones huérfanas corriendo
    ///   en paralelo con el siguiente intento.
    /// - Si el servicio se está deteniendo (token del llamador cancelado) no se reintenta.
    /// - Opcionalmente reintenta cuando la operación termina sin excepción pero con un resultado no válido
    ///   (por ejemplo, una subida FTP que devuelve false).
    /// </summary>
    internal class ResilientExecutor
    {
        private readonly ILogger<ResilientExecutor> _logger;
        private readonly int _maxRetries;
        private readonly int _baseDelayMs;
        private readonly int _jitterMs;
        private readonly int _timeoutPerAttemptSec;

        public ResilientExecutor(ILogger<ResilientExecutor> logger, IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            var section = configuration.GetSection("FtpResilience");
            _maxRetries = Math.Max(0, section.GetValue<int>("UploadRetryCount", 3));
            _baseDelayMs = Math.Max(0, section.GetValue<int>("UploadBaseDelayMs", 500));
            _jitterMs = Math.Max(0, section.GetValue<int>("JitterMs", 200));
            _timeoutPerAttemptSec = Math.Max(1, section.GetValue<int>("TimeoutPerAttemptSec", 30));
        }

        public Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken ct = default)
            => ExecuteAsync(operation, retryWhen: null, ct);

        /// <param name="retryWhen">
        /// Condición sobre el resultado que indica que el intento falló aunque no lanzó excepción.
        /// Si después del último intento se sigue cumpliendo, se devuelve ese último resultado.
        /// </param>
        public async Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> operation, Func<T, bool>? retryWhen, CancellationToken ct = default)
        {
            if (operation == null) throw new ArgumentNullException(nameof(operation));

            var totalAttempts = _maxRetries + 1;
            Exception? lastException = null;

            for (var attempt = 1; attempt <= totalAttempts; attempt++)
            {
                ct.ThrowIfCancellationRequested();

                var outcome = await RunAttemptAsync(operation, attempt, ct);
                if (outcome.Exception == null)
                {
                    if (retryWhen == null || !retryWhen(outcome.Result!))
                        return outcome.Result!;

                    _logger.LogWarning("ResilientExecutor: attempt {Attempt} of {TotalAttempts} returned an unsuccessful result", attempt, totalAttempts);
                    if (attempt == totalAttempts)
                    {
                        _logger.LogError("ResilientExecutor: operation unsuccessful after {Attempts} attempts", totalAttempts);
                        return outcome.Result!;
                    }
                }
                else
                {
                    lastException = outcome.Exception;
                }

                if (attempt < totalAttempts)
                    await DelayBeforeNextAttemptAsync(attempt, ct);
            }

            _logger.LogError(lastException, "ResilientExecutor: operation failed after {Attempts} attempts", totalAttempts);
            throw lastException ?? new InvalidOperationException("ResilientExecutor: unknown error");
        }

        public async Task ExecuteAsync(Func<CancellationToken, Task> operation, CancellationToken ct = default)
        {
            await ExecuteAsync<object?>(async token => { await operation(token); return null; }, ct);
        }

        /// <summary>
        /// Ejecuta un intento con su propio timeout. Devuelve el resultado o la excepción del intento;
        /// solo relanza cuando la cancelación viene del llamador (el servicio se está deteniendo).
        /// </summary>
        private async Task<(T? Result, Exception? Exception)> RunAttemptAsync<T>(Func<CancellationToken, Task<T>> operation, int attempt, CancellationToken ct)
        {
            using var attemptCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            attemptCts.CancelAfter(TimeSpan.FromSeconds(_timeoutPerAttemptSec));

            try
            {
                var result = await operation(attemptCts.Token);
                return (result, null);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw;
            }
            catch (OperationCanceledException oce)
            {
                _logger.LogWarning(oce, "ResilientExecutor: attempt {Attempt} timed out after {TimeoutSec}s", attempt, _timeoutPerAttemptSec);
                return (default, oce);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "ResilientExecutor: attempt {Attempt} failed", attempt);
                return (default, ex);
            }
        }

        private async Task DelayBeforeNextAttemptAsync(int attempt, CancellationToken ct)
        {
            var delay = TimeSpan.FromMilliseconds(Math.Pow(2, attempt) * _baseDelayMs + Random.Shared.Next(0, _jitterMs + 1));
            _logger.LogInformation("ResilientExecutor: delaying {Delay} before next attempt", delay);
            await Task.Delay(delay, ct);
        }
    }
}
