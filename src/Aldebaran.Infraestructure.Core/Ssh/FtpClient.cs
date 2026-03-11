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

                        // Now place temp into final destination depending on overwrite flag
                        var maxRenameAttempts = 3;
                        if (overwrite)
                        {
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
                        else
                        {
                            // overwrite == false: if target does not exist, try rename to it
                            if (!await ftp.FileExists(remotePath, ct))
                            {
                                try
                                {
                                    await ftp.Rename(tempRemote, remotePath, ct);
                                    return true;
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogWarning(ex, "Rename to initial non-overwrite name failed {Temp} -> {Final}", tempRemote, remotePath);
                                }
                            }

                            // Otherwise generate suffixed candidate names and try to rename
                            var dir = Path.GetDirectoryName(remotePath) ?? string.Empty;
                            var fileNameOnly = Path.GetFileName(remotePath);
                            var nameOnly = Path.GetFileNameWithoutExtension(fileNameOnly);
                            var ext = Path.GetExtension(fileNameOnly);

                            for (int attempt = 1; attempt <= maxRenameAttempts; attempt++)
                            {
                                var suffix = DateTime.UtcNow.ToString("yyyyMMddHHmmss") + (attempt > 1 ? $"_{attempt}" : string.Empty);
                                var candidate = Path.Combine(dir, nameOnly + "_" + suffix + ext).Replace('\\', '/');
                                try
                                {
                                    if (!await ftp.FileExists(candidate, ct))
                                    {
                                        await ftp.Rename(tempRemote, candidate, ct);
                                        return true;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogWarning(ex, "Rename attempt to candidate failed {Temp} -> {Candidate}", tempRemote, candidate);
                                }
                                try { await Task.Delay(TimeSpan.FromSeconds(1 * attempt), ct); } catch { }
                            }

                            try { if (await ftp.FileExists(tempRemote, ct)) await ftp.DeleteFile(tempRemote, ct); } catch { }
                            return false;
                        }
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
