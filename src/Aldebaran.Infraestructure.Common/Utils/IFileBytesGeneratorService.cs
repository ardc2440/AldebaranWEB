namespace Aldebaran.Infraestructure.Common.Utils
{
    public interface IFileBytesGeneratorService
    {
        Task<byte[]> GetPdfBytes(string content, bool landscape = false);
        Task<byte[]> GetExcelBytes<T>(List<T> data);

        /// <summary>
        /// Excel con filas agrupadas (outline): cada padre (nivel 0) seguido de sus hijos (nivel 1),
        /// contraídos y expandibles con "+". Hoja sin protección. Imágenes ([ExcelImage]) solo en los padres.
        /// </summary>
        Task<byte[]> GetExcelBytesWithChildRows<T>(List<T> parents, Func<T, IEnumerable<T>> childrenSelector);
        Task<string> GetExcelTempFile<T>(List<T> data);
        Task<string> GetPdfTempFile(string content, bool landscape = false);
        Task<byte[]> GetCsvBytes<T>(List<T> data);
    }
}
