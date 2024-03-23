using Aldebaran.DataAccess.Entities;


namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface ICustomerOrderReportRepository
    {
        Task<IEnumerable<CustomerOrderReport>> GetCustomerOrderReportDataAsync(CancellationToken ct = default);
    }
}
