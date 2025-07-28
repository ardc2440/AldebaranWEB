using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IVisualizedAutomaticCustomerInProcessModificationService
    {
        Task AddAsync(VisualizedAutomaticCustomerOrderInProcessModification item, CancellationToken ct = default);
    }
}
