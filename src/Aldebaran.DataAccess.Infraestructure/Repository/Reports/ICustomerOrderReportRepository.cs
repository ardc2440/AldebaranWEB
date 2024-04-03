using Aldebaran.DataAccess.Entities.Reports;


namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public interface ICustomerOrderReportRepository
    {
        Task<IEnumerable<CustomerOrderReport>> GetCustomerOrderReportDataAsync(string filter = "", CancellationToken ct = default);

        Task<IEnumerable<CustomerOrderExport>> GetCustomerOrderExportDataAsync(string filter, CancellationToken ct = default);
    }
}
