using Aldebaran.Infraestructure.Core.Model;
using FluentFTP.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;

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

        public async Task<bool> UploadFileAsync(byte[] fileBytes, string fileName, bool overwrite = true)
        {
            using (FluentFTP.AsyncFtpClient ftp = new FluentFTP.AsyncFtpClient(_settings.Host, _settings.Port))
            {
                ftp.Credentials = new NetworkCredential(_settings.Username, _settings.Password);
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
    }
}
