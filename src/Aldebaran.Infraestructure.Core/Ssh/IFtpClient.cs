namespace Aldebaran.Infraestructure.Core.Ssh
{
    public interface IFtpClient
    {
        Task<bool> UploadFileFromPathAsync(string localPath, string remotePath, bool overwrite = true, CancellationToken ct = default);
        Task<bool> UploadFileFromPathAsync(string localPath, string remotePath, string host, int port, string username, string password, bool overwrite = true, CancellationToken ct = default);
    }
}
