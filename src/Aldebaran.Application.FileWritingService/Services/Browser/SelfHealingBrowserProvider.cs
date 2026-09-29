using Microsoft.Extensions.Logging;
using PuppeteerSharp;

namespace Aldebaran.Application.FileWritingService.Services.Browser
{
    /// <summary>
    /// Proveedor de Chromium exclusivo del FileWritingService.
    /// A diferencia del BrowserProvider compartido, verifica que la instancia siga viva
    /// y la relanza si Chromium se cerró o perdió la conexión (por ejemplo, tras un OutOfMemory).
    /// </summary>
    internal sealed class SelfHealingBrowserProvider : IResettableBrowserProvider, IAsyncDisposable
    {
        private static readonly TimeSpan CloseTimeout = TimeSpan.FromSeconds(15);

        private readonly ILogger<SelfHealingBrowserProvider> _logger;
        private readonly SemaphoreSlim _lock = new(1, 1);
        private IBrowser? _browser;
        private bool _browserDownloaded;

        public SelfHealingBrowserProvider(ILogger<SelfHealingBrowserProvider> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IBrowser> GetBrowserAsync()
        {
            var current = _browser;
            if (IsAlive(current))
                return current!;

            await _lock.WaitAsync();
            try
            {
                if (IsAlive(_browser))
                    return _browser!;

                await DiscardBrowserAsync();
                _browser = await LaunchBrowserAsync();
                return _browser;
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task ResetAsync()
        {
            await _lock.WaitAsync();
            try
            {
                await DiscardBrowserAsync();
            }
            finally
            {
                _lock.Release();
            }
        }

        public async ValueTask DisposeAsync()
        {
            await ResetAsync();
            _lock.Dispose();
        }

        private static bool IsAlive(IBrowser? browser) =>
            browser is { IsConnected: true, IsClosed: false };

        private async Task<IBrowser> LaunchBrowserAsync()
        {
            await EnsureBrowserDownloadedAsync();

            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                Args = new[]
                {
                    "--no-sandbox",
                    "--disable-dev-shm-usage",
                    "--allow-file-access-from-files"
                }
            });

            browser.Disconnected += OnBrowserDisconnected;
            _logger.LogInformation("Chromium iniciado. ProcessId: {ProcessId}", browser.Process?.Id);
            return browser;
        }

        private async Task EnsureBrowserDownloadedAsync()
        {
            if (_browserDownloaded)
                return;

            await new BrowserFetcher().DownloadAsync();
            _browserDownloaded = true;
        }

        private async Task DiscardBrowserAsync()
        {
            var browser = _browser;
            _browser = null;
            if (browser == null)
                return;

            browser.Disconnected -= OnBrowserDisconnected;
            var processId = browser.Process?.Id;

            try
            {
                await browser.CloseAsync().WaitAsync(CloseTimeout);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No fue posible cerrar Chromium de forma ordenada. ProcessId: {ProcessId}", processId);
                KillProcess(browser);
            }
            finally
            {
                try { browser.Dispose(); } catch (Exception ex) { _logger.LogDebug(ex, "Error liberando la instancia de Chromium"); }
            }

            _logger.LogWarning("Instancia de Chromium descartada. ProcessId: {ProcessId}", processId);
        }

        private void KillProcess(IBrowser browser)
        {
            try
            {
                var process = browser.Process;
                if (process != null && !process.HasExited)
                    process.Kill(entireProcessTree: true);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No fue posible terminar el proceso de Chromium");
            }
        }

        private void OnBrowserDisconnected(object? sender, EventArgs e)
        {
            _logger.LogWarning("Chromium se desconectó. Se relanzará en la siguiente generación de PDF.");
        }
    }
}
