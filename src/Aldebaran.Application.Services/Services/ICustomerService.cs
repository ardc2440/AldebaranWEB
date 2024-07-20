using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface ICustomerService
    {
        Task<Customer?> FindAsync(int customerId, CancellationToken ct = default);
        Task<bool> ExistsByName(string name, CancellationToken ct = default);
        Task<bool> ExistsByIdentificationNumber(string identificationNumber, CancellationToken ct = default);
        Task<(IEnumerable<Customer> Customers, int)> GetAsync(int? skip=null, int? top = null, CancellationToken ct = default);
        Task<(IEnumerable<Customer>, int)> GetAsync(int skip, int top, string filter, CancellationToken ct = default);
        Task AddAsync(Customer customer, CancellationToken ct = default);
        Task UpdateAsync(int customerId, Customer customer, CancellationToken ct = default);
        Task DeleteAsync(int customerId, CancellationToken ct = default);
    }

}
