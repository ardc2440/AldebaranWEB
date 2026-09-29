namespace Aldebaran.Application.FileWritingService.Services.Pdf
{
    /// <summary>
    /// Genera el PDF del inventario en un archivo temporal a partir del HTML ya renderizado.
    /// </summary>
    public interface IInventoryPdfFileGenerator
    {
        /// <returns>Ruta del archivo PDF temporal. El llamador es responsable de eliminarlo.</returns>
        Task<string> GenerateAsync(string html, bool landscape, CancellationToken ct);
    }
}
