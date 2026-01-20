namespace Aldebaran.Infraestructure.Core.Ssh
{
    public interface IFtpClient
    {
        Task<bool> UploadFileAsync(byte[] fileBytes, string fileName, bool overwrite = true);
        Task<bool> UploadFileAsync(byte[] fileBytes, string fileName, string host, int port, string username, string password, bool overwrite = true);
    }
}
