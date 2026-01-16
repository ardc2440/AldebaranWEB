using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services.Services
{
    public interface IFtpWritingConnectionService
    {
        Task<FtpWritingConnection> CreateAsync(FtpWritingConnection model, CancellationToken ct = default);
        Task<FtpWritingConnection> ChangeActivationAsync(int id, bool active, CancellationToken ct = default);
        Task<IEnumerable<FtpWritingConnection>> GetAllAsync(CancellationToken ct = default);
        Task<FtpWritingConnection> UpdateAsync(FtpWritingConnection item, CancellationToken ct = default);
        Task<FtpWritingConnection> GetByIdAsync(int id, CancellationToken ct = default);

    }
}