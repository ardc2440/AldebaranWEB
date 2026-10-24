using System.Globalization;
using Aldebaran.Web.Shared.ViewModel;
using ReportFilter = Aldebaran.Application.Services.Models.Reports.CustomerOrderReportFilter;

namespace Aldebaran.Web.Pages.ReportPages
{
    /// <summary>
    /// Convierte los filtros de pantalla de los reportes de pedidos en el filtro del caso de uso (sin armar SQL)
    /// y en la cadena de consulta de las exportaciones.
    /// </summary>
    public static class CustomerOrderReportFilterExtensions
    {
        public static ReportFilter ToReportFilter(this Customer_Orders.ViewModel.CustomerOrderFilter filter) =>
            Build(filter.OrderNumber, filter.CreationDate, filter.OrderDate, filter.EstimatedDeliveryDate, filter.StatusDocumentTypeId,
                  filter.CustomerId, filter.ItemReferences.Select(s => s.ReferenceId), filter.IsSpecialOrder);

        public static ReportFilter ToReportFilter(this Automatic_Customer_Order_In_Process_Creation.ViewModel.BackOrderFilter filter) =>
            Build(filter.OrderNumber, filter.CreationDate, filter.OrderDate, filter.EstimatedDeliveryDate, filter.StatusDocumentTypeId,
                  filter.CustomerId, filter.ItemReferences.Select(s => s.ReferenceId), filter.IsSpecialOrder);

        /// <summary>Parámetros de consulta para los controladores de exportación (vacío = sin filtros).</summary>
        public static string ToQueryString(this ReportFilter filter)
        {
            var values = new List<string>();
            Add(values, nameof(ReportFilter.OrderNumber), filter.OrderNumber);
            Add(values, nameof(ReportFilter.CreationDateFrom), filter.CreationDateFrom);
            Add(values, nameof(ReportFilter.CreationDateTo), filter.CreationDateTo);
            Add(values, nameof(ReportFilter.OrderDateFrom), filter.OrderDateFrom);
            Add(values, nameof(ReportFilter.OrderDateTo), filter.OrderDateTo);
            Add(values, nameof(ReportFilter.EstimatedDeliveryDateFrom), filter.EstimatedDeliveryDateFrom);
            Add(values, nameof(ReportFilter.EstimatedDeliveryDateTo), filter.EstimatedDeliveryDateTo);
            Add(values, nameof(ReportFilter.StatusDocumentTypeId), filter.StatusDocumentTypeId?.ToString(CultureInfo.InvariantCulture));
            Add(values, nameof(ReportFilter.CustomerId), filter.CustomerId?.ToString(CultureInfo.InvariantCulture));
            Add(values, nameof(ReportFilter.ReferenceIds), filter.ReferenceIds);
            Add(values, nameof(ReportFilter.IsSpecialOrder), filter.IsSpecialOrder?.ToString().ToLowerInvariant());
            return values.Count == 0 ? string.Empty : "?" + string.Join("&", values);
        }

        private static ReportFilter Build(string orderNumber, DateRange creationDate, DateRange orderDate, DateRange estimatedDeliveryDate,
                                          short? statusDocumentTypeId, int? customerId, IEnumerable<int> referenceIds, bool? isSpecialOrder)
        {
            var references = referenceIds.ToList();
            return new ReportFilter
            {
                OrderNumber = string.IsNullOrWhiteSpace(orderNumber) ? null : orderNumber.Trim(),
                CreationDateFrom = creationDate.StartDate,
                CreationDateTo = creationDate.StartDate.HasValue ? creationDate.EndDate : null,
                OrderDateFrom = orderDate.StartDate,
                OrderDateTo = orderDate.StartDate.HasValue ? orderDate.EndDate : null,
                EstimatedDeliveryDateFrom = estimatedDeliveryDate.StartDate,
                EstimatedDeliveryDateTo = estimatedDeliveryDate.StartDate.HasValue ? estimatedDeliveryDate.EndDate : null,
                StatusDocumentTypeId = statusDocumentTypeId,
                CustomerId = customerId,
                ReferenceIds = references.Count > 0 ? string.Join(",", references) : null,
                IsSpecialOrder = isSpecialOrder
            };
        }

        private static void Add(List<string> values, string name, DateTime? value) =>
            Add(values, name, value?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));

        private static void Add(List<string> values, string name, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                values.Add($"{name}={Uri.EscapeDataString(value)}");
        }
    }
}
