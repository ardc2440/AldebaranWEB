using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface ICustomerService
    {
        Task<Customer?> FindAsync(int customerId, CancellationToken ct = default);
        Task<IEnumerable<Customer>> GetAsync(CancellationToken ct = default);
        Task<IEnumerable<Customer>> GetAsync(string filter, CancellationToken ct = default);
    }

}
