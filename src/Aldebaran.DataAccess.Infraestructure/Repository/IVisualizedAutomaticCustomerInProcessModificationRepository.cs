using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IVisualizedAutomaticCustomerInProcessModificationRepository
    {
        Task AddAsync(VisualizedAutomaticCustomerOrderInProcessModification item, CancellationToken ct = default);
    }
}
