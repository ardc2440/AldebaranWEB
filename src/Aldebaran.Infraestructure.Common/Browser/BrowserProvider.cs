using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.Infraestructure.Common.Browser
{
    public class BrowserProvider : IBrowserProvider, IAsyncDisposable
    {
        private IBrowser? _browser;
        private readonly SemaphoreSlim _lock = new(1, 1);
        private bool _initialized = false;

        public async Task<IBrowser> GetBrowserAsync()
        {
            if (_browser != null)
                return _browser;

            await _lock.WaitAsync();
            try
            {
                if (_browser == null)
                {
                    await InitializeAsync();
                }
            }
            finally
            {
                _lock.Release();
            }

            return _browser!;
        }

        private async Task InitializeAsync()
        {
            if (_initialized) return;

            // 🔴 Se ejecuta UNA sola vez en toda la vida del servicio
            await new BrowserFetcher().DownloadAsync();

            _browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                Args = new[]
                {
                "--no-sandbox",
                "--disable-dev-shm-usage"
            }
            });

            _initialized = true;
        }

        public async ValueTask DisposeAsync()
        {
            if (_browser != null)
            {
                await _browser.CloseAsync();
                _browser.Dispose();
            }
        }
    }
}
