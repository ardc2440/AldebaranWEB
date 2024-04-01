using Aldebaran.DataAccess.Entities.Reports;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public interface ICustomerSaleReportRepository
    {
        Task<IEnumerable<CustomerSaleReport>> GetCustomerSaleReportDataAsync(string filter = "", CancellationToken ct = default);
    }
}
