using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IFtpWritingConnectionRepository
    {
        Task<FtpWritingConnection> AddAsync(FtpWritingConnection connection, CancellationToken ct = default);

        Task<FtpWritingConnection> GetByIdAsync(int Id, CancellationToken ct = default);

        Task<IEnumerable<FtpWritingConnection>> GetAllAsync(CancellationToken ct = default);

        Task<FtpWritingConnection> UpdateAsync(FtpWritingConnection connection, CancellationToken ct = default);

        Task<FtpWritingConnection> ChangeActivationAsync(int Id, bool active, CancellationToken ct = default);

    }
}