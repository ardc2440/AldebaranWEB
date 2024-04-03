namespace Aldebaran.Infraestructure.Common.Utils
{
    public interface IFileBytesGeneratorService
    {
        Task<byte[]> GetPdfBytes(string content, bool landscape = false);
        Task<byte[]> GetExcelBytes<T>(List<T> data);
        Task<byte[]> GetCsvBytes<T>(List<T> data);
    }
}
