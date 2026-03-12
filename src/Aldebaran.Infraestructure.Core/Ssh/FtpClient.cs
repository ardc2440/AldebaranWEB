using Aldebaran.Infraestructure.Core.Model;
using FluentFTP.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
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

                    // Upload to temporary remote file first
                    var tempRemote = remotePath + ".uploading." + Guid.NewGuid().ToString("N");
                    try
                    {
                        using (var fs = File.OpenRead(localPath))
                        {
                            var uploadResult = await ftp.UploadStream(fs, tempRemote, FluentFTP.FtpRemoteExists.Overwrite, true, null, ct);
                            if (!uploadResult.IsSuccess())
                            {
                                _logger.LogWarning("Upload to temp remote failed for {RemoteTemp}", tempRemote);
                                try { if (await ftp.FileExists(tempRemote, ct)) await ftp.DeleteFile(tempRemote, ct); } catch { }
                                return false;
                            }
                        }

                        // Place temp into final destination: always attempt to delete existing remotePath and rename temp into place.
                        var maxRenameAttempts = 3;
                        for (int attempt = 1; attempt <= maxRenameAttempts; attempt++)
                        {
                            try
                            {
                                if (await ftp.FileExists(remotePath, ct))
                                {
                                    try { await ftp.DeleteFile(remotePath, ct); } catch (Exception delEx) { _logger.LogWarning(delEx, "Could not delete existing remote file before rename (attempt {Attempt}) {Remote}", attempt, remotePath); }
                                }

                                await ftp.Rename(tempRemote, remotePath, ct);
                                return true;
                            }
                            catch (OperationCanceledException)
                            {
                                throw;
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, "Rename attempt {Attempt} failed for {Temp} -> {Final}", attempt, tempRemote, remotePath);
                                if (attempt == maxRenameAttempts)
                                {
                                    try { if (await ftp.FileExists(tempRemote, ct)) await ftp.DeleteFile(tempRemote, ct); } catch { }
                                    return false;
                                }
                                try { await Task.Delay(TimeSpan.FromSeconds(1 * attempt), ct); } catch { }
                            }
                        }

                        return false;
                    }
                    finally
                    {
                        try { if (await ftp.FileExists(tempRemote, ct)) await ftp.DeleteFile(tempRemote, ct); } catch { }
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
