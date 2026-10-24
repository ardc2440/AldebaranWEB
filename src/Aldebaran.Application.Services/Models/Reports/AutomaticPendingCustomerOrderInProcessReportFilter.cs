namespace Aldebaran.Application.Services.Models.Reports
{
    /// <summary>Filtros del reporte de pedidos pendientes de traslado automático a proceso.</summary>
    public class AutomaticPendingCustomerOrderInProcessReportFilter
    {
        public string? OrderNumber { get; set; }
        public DateTime? OrderDateFrom { get; set; }
        public DateTime? OrderDateTo { get; set; }
        public DateTime? ProcessDateFrom { get; set; }
        public DateTime? ProcessDateTo { get; set; }
        public int? CustomerId { get; set; }
        public short? StatusDocumentTypeId { get; set; }
        /// <summary>Ids de referencia separados por coma; vacío = todas.</summary>
        public string? ReferenceIds { get; set; }
        /// <summary>Pedido especial: true = Sí, false = No, null = Todos.</summary>
        public bool? IsSpecialOrder { get; set; }
    }
}
