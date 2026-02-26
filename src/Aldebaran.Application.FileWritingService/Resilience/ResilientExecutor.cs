using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Aldebaran.Application.FileWritingService.Resilience
{
    internal class ResilientExecutor
    {
        private readonly ILogger<ResilientExecutor> _logger;
        private readonly int _maxRetries;
        private readonly int _baseDelayMs;
        private readonly int _jitterMs;
        private readonly int _timeoutPerAttemptSec;
        private readonly Random _jitterer = new();

        public ResilientExecutor(ILogger<ResilientExecutor> logger, IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            var section = configuration.GetSection("FtpResilience");
            _maxRetries = section.GetValue<int>("UploadRetryCount", 3);
            _baseDelayMs = section.GetValue<int>("UploadBaseDelayMs", 500);
            _jitterMs = section.GetValue<int>("JitterMs", 200);
            _timeoutPerAttemptSec = section.GetValue<int>("TimeoutPerAttemptSec", 30);
        }

        public async Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken ct = default)
        {
            if (operation == null) throw new ArgumentNullException(nameof(operation));

            int attempt = 0;
            Exception? lastEx = null;
            while (attempt <= _maxRetries)
            {
                CancellationTokenSource? cts = null;
                try
                {
                    attempt++;
                    cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                    cts.CancelAfter(TimeSpan.FromSeconds(_timeoutPerAttemptSec));

                    var operationTask = operation(cts.Token);
                    var delayTask = Task.Delay(TimeSpan.FromSeconds(_timeoutPerAttemptSec), ct);

                    var completed = await Task.WhenAny(operationTask, delayTask);
                    if (completed != operationTask)
                    {
                        // timed out
                        lastEx = new OperationCanceledException("ResilientExecutor: operation timed out");
                        _logger.LogWarning(lastEx, "ResilientExecutor: attempt {Attempt} timed out", attempt);
                        try { cts.Cancel(); } catch { }
                    }
                    else
                    {
                        // operation completed (may fault)
                        return await operationTask;
                    }
                }
                catch (OperationCanceledException oce)
                {
                    lastEx = oce;
                    _logger.LogWarning(oce, "ResilientExecutor: attempt {Attempt} cancelled/timeout", attempt);
                }
                catch (Exception ex)
                {
                    lastEx = ex;
                    _logger.LogWarning(ex, "ResilientExecutor: attempt {Attempt} failed", attempt);
                }
                finally
                {
                    if (cts != null)
                        cts.Dispose();
                }

                if (attempt > _maxRetries)
                    break;

                var delay = TimeSpan.FromMilliseconds(Math.Pow(2, attempt) * _baseDelayMs + _jitterer.Next(0, _jitterMs));
                _logger.LogInformation("ResilientExecutor: delaying {Delay} before next attempt", delay);
                try { await Task.Delay(delay, ct); } catch { break; }
            }

            _logger.LogError(lastEx, "ResilientExecutor: operation failed after {Attempts} attempts", _maxRetries);
            throw lastEx ?? new InvalidOperationException("ResilientExecutor: unknown error");
        }

        public async Task ExecuteAsync(Func<CancellationToken, Task> operation, CancellationToken ct = default)
        {
            await ExecuteAsync<object?>(async token => { await operation(token); return null; }, ct);
        }
    }
}
