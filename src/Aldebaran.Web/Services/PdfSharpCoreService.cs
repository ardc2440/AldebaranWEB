using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace Aldebaran.Web
{
    public class PdfSharpCoreService : IPdfService
    {
        public async Task<byte[]> GetBytes(string content)
        {
            // Iniciar una instancia de Chromium a través de PuppeteerSharp
            await new BrowserFetcher().DownloadAsync();
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });
            var page = await browser.NewPageAsync();
            await page.SetContentAsync(content);

            var pdfBytes = await page.PdfDataAsync(new PdfOptions
            {
                Format = PaperFormat.A4,
                PrintBackground = true,
                MarginOptions = new MarginOptions
                {
                    Top = "2cm",
                    Bottom = "2cm",
                    Left = "2cm",
                    Right = "2cm"
                },
                DisplayHeaderFooter = true,
                FooterTemplate = @"
                <div style='font-size: 10px; color: #888; text-align: center; display:block; width:100%;'>
                    <span class='pageNumber'></span> de <span class='totalPages'></span>
                </div>",
            });

            // Cerrar el navegador Chromium
            await browser.CloseAsync();

            return pdfBytes;
        }

    }
}