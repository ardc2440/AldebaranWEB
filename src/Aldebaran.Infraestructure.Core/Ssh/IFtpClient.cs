namespace Aldebaran.Infraestructure.Core.Ssh
{
    public interface IFtpClient
    {
        Task<bool> UploadFileAsync(byte[] fileBytes, string fileName, bool overwrite = true);
    }
}
