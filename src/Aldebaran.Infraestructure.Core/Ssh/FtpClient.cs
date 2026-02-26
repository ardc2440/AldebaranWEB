using Aldebaran.Infraestructure.Core.Model;
using FluentFTP.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Threading;
using System.IO;

namespace Aldebaran.Infraestructure.Core.Ssh
{
    public class FtpClient : IFtpClient
    {
        private readonly ILogger<FtpClient> _logger;
        private readonly FtpSettings _settings;
        public FtpClient(ILogger<FtpClient> logger, IOptions<FtpSettings> settings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(ILogger<FtpClient>));
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(IOptions<FtpSettings>));
        }

        public Task<bool> UploadFileAsync(byte[] fileBytes, string fileName, bool overwrite = true)
        {
            return UploadFileAsync(fileBytes, fileName, _settings.Host, _settings.Port, _settings.Username, _settings.Password, overwrite);
        }

        public async Task<bool> UploadFileAsync(byte[] fileBytes, string fileName, string host, int port, string username, string password, bool overwrite = true)
        {
            using (FluentFTP.AsyncFtpClient ftp = new FluentFTP.AsyncFtpClient(host, port))
            {
                ftp.Credentials = new NetworkCredential(username, password);
                try
                {
                    await ftp.AutoConnect();
                    var result = await ftp.UploadBytes(fileBytes, fileName, overwrite ? FluentFTP.FtpRemoteExists.Overwrite : FluentFTP.FtpRemoteExists.Resume, true);
                    return result.IsSuccess();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error al subir archivo al ftp: {ex.Message}");
                    return false;
                }
                finally
                {
                    await ftp.Disconnect();
                }
            }
        }

        public Task<bool> UploadFileFromPathAsync(string localPath, string remotePath, bool overwrite = true, CancellationToken ct = default)
        {
            return UploadFileFromPathAsync(localPath, remotePath, _settings.Host, _settings.Port, _settings.Username, _settings.Password, overwrite, ct);
        }

        public async Task<bool> UploadFileFromPathAsync(string localPath, string remotePath, string host, int port, string username, string password, bool overwrite = true, CancellationToken ct = default)
        {
            using (FluentFTP.AsyncFtpClient ftp = new FluentFTP.AsyncFtpClient(host, port))
            {
                ftp.Credentials = new NetworkCredential(username, password);
                try
                {
                    // configure timeouts if available
                    try
                    {
                        ftp.Config.ConnectTimeout = 10000;
                        ftp.Config.DataConnectionConnectTimeout = 10000;
                        ftp.Config.ReadTimeout = 30000;
                    }
                    catch { }

                    await ftp.AutoConnect(ct);
                    using (var fs = File.OpenRead(localPath))
                    {
                        var result = await ftp.UploadStream(fs, remotePath, overwrite ? FluentFTP.FtpRemoteExists.Overwrite : FluentFTP.FtpRemoteExists.Resume, true, null, ct);
                        return result.IsSuccess();
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("Upload cancelled for {Host}:{File}", host, remotePath);
                    return false;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error al subir archivo al ftp: {ex.Message}");
                    return false;
                }
                finally
                {
                    try { await ftp.Disconnect(ct); } catch { }
                }
            }
        }
    }
}
