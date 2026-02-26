namespace Aldebaran.Infraestructure.Common.Utils
{
    public interface IFileBytesGeneratorService
    {
        Task<byte[]> GetPdfBytes(string content, bool landscape = false);
        Task<byte[]> GetExcelBytes<T>(List<T> data);
        Task<string> GetExcelTempFile<T>(List<T> data);
        Task<string> GetPdfTempFile(string content, bool landscape = false);
        Task<byte[]> GetCsvBytes<T>(List<T> data);
    }
}
