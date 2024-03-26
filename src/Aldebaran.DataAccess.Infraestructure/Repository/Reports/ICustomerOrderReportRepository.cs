using Aldebaran.DataAccess.Entities.Reports;


namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public interface ICustomerOrderReportRepository
    {
        Task<IEnumerable<CustomerOrderReport>> GetCustomerOrderReportDataAsync(CancellationToken ct = default);
    }
}
