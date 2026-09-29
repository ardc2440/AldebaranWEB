using Aldebaran.Application.FileWritingService.Services.Browser;
using Aldebaran.Application.FileWritingService.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using System.Text;

namespace Aldebaran.Application.FileWritingService.Services.Pdf
{
    /// <summary>
    /// Genera el PDF sin enviar el HTML por el canal de Chromium: el HTML se escribe a un archivo
    /// temporal y Chromium lo abre desde disco. Si Chromium falla, se descarta para que la
    /// siguiente ejecución use una instancia nueva.
    /// </summary>
    internal sealed class InventoryPdfFileGenerator : IInventoryPdfFileGenerator
    {
        private const string FooterTemplate = @"
            <div style='font-size: 10px; color: #888; text-align: center; display:block; width:100%;'>
                <span class='pageNumber'></span> de <span class='totalPages'></span>
            </div>";

        private readonly IResettableBrowserProvider _browserProvider;
        private readonly PdfGenerationOptions _options;
        private readonly ILogger<InventoryPdfFileGenerator> _logger;

        public InventoryPdfFileGenerator(IResettableBrowserProvider browserProvider, IOptions<PdfGenerationOptions> options, ILogger<InventoryPdfFileGenerator> logger)
        {
            _browserProvider = browserProvider ?? throw new ArgumentNullException(nameof(browserProvider));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> GenerateAsync(string html, bool landscape, CancellationToken ct)
        {
            var tempDir = GetTempDirectory();
            var htmlPath = Path.Combine(tempDir, $"inventory_{Guid.NewGuid()}.html");
            var pdfPath = Path.Combine(tempDir, $"inventory_{Guid.NewGuid()}.pdf");

            try
            {
                await File.WriteAllTextAsync(htmlPath, html, new UTF8Encoding(false), ct);
                await RenderPdfAsync(html, htmlPath, pdfPath, landscape);
                return pdfPath;
            }
            catch (Exception ex) when (ex is not OperationCanceledException || !ct.IsCancellationRequested)
            {
                _logger.LogError(ex, "Falló la generación del PDF; se descarta la instancia de Chromium");
                await _browserProvider.ResetAsync();
                TryDelete(pdfPath);
                throw;
            }
            finally
            {
                TryDelete(htmlPath);
            }
        }

        /// <summary>
        /// Intenta generar el PDF abriendo el HTML desde disco (sin enviarlo por el canal de Chromium).
        /// Si eso falla y Chromium sigue vivo, usa el método anterior (SetContentAsync) para no perder el archivo.
        /// </summary>
        private async Task RenderPdfAsync(string html, string htmlPath, string pdfPath, bool landscape)
        {
            var browser = await _browserProvider.GetBrowserAsync();
            try
            {
                await RenderFromFileAsync(browser, htmlPath, pdfPath, landscape);
            }
            catch (Exception ex) when (browser.IsConnected)
            {
                _logger.LogWarning(ex, "No fue posible generar el PDF desde archivo; se usa la carga directa del contenido");
                TryDelete(pdfPath);
                await RenderFromContentAsync(browser, html, pdfPath, landscape);
            }
        }

        private async Task RenderFromFileAsync(IBrowser browser, string htmlPath, string pdfPath, bool landscape)
        {
            var page = await browser.NewPageAsync();
            try
            {
                await page.GoToAsync(new Uri(htmlPath).AbsoluteUri, new NavigationOptions
                {
                    Timeout = _options.LoadTimeoutSeconds * 1000,
                    WaitUntil = new[] { WaitUntilNavigation.Load }
                });

                await WritePdfAsync(page, pdfPath, landscape);
            }
            finally
            {
                await ClosePageAsync(page);
            }
        }

        private async Task RenderFromContentAsync(IBrowser browser, string html, string pdfPath, bool landscape)
        {
            var page = await browser.NewPageAsync();
            try
            {
                await page.SetContentAsync(html, new NavigationOptions
                {
                    Timeout = _options.LoadTimeoutSeconds * 1000,
                    WaitUntil = new[] { WaitUntilNavigation.Load }
                });

                await WritePdfAsync(page, pdfPath, landscape);
            }
            finally
            {
                await ClosePageAsync(page);
            }
        }

        private Task WritePdfAsync(IPage page, string pdfPath, bool landscape) =>
            page.PdfAsync(pdfPath, BuildPdfOptions(landscape))
                .WaitAsync(TimeSpan.FromSeconds(_options.PdfTimeoutSeconds));

        private static PdfOptions BuildPdfOptions(bool landscape) => new()
        {
            Format = PaperFormat.A4,
            Landscape = landscape,
            PrintBackground = true,
            MarginOptions = new MarginOptions
            {
                Top = "2cm",
                Bottom = "2cm",
                Left = "2cm",
                Right = "2cm"
            },
            DisplayHeaderFooter = true,
            FooterTemplate = FooterTemplate
        };

        private async Task ClosePageAsync(IPage page)
        {
            try
            {
                await page.CloseAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No fue posible cerrar la página de Chromium");
            }
        }

        private static string GetTempDirectory()
        {
            var tempDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? ".", "temp");
            Directory.CreateDirectory(tempDir);
            return tempDir;
        }

        private void TryDelete(string path)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No fue posible eliminar el archivo temporal {Path}", path);
            }
        }
    }
}
