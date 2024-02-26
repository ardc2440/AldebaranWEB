namespace Aldebaran.Web
{
    public interface IPdfService
    {
        Task<byte[]> GetBytes(string content);
    }
}