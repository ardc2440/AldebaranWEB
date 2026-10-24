namespace Aldebaran.DataAccess.Entities.Reports
{
    /// <summary>Filtros del reporte de asignación automática de referencias a pedidos.</summary>
    public class AutomaticAssigmentReportFilter
    {
        public string? DocumentType { get; set; }
        public string? DocumentNumber { get; set; }
        public int? ProviderId { get; set; }
        public string? ProformaNumber { get; set; }
        public string? ImportNumber { get; set; }
        public DateTime? ReceiptDateFrom { get; set; }
        public DateTime? ReceiptDateTo { get; set; }
        public DateTime? ConfirmedDateFrom { get; set; }
        public DateTime? ConfirmedDateTo { get; set; }
        public string? CustomerOrderNumber { get; set; }
        public int? CustomerId { get; set; }
        public DateTime? OrderDateFrom { get; set; }
        public DateTime? OrderDateTo { get; set; }
        public DateTime? EstimatedDeliveryDateFrom { get; set; }
        public DateTime? EstimatedDeliveryDateTo { get; set; }
        public short? StatusDocumentTypeId { get; set; }
        /// <summary>Ids de referencia separados por coma; vacío = todas.</summary>
        public string? ReferenceIds { get; set; }
        /// <summary>Pedido especial: true = Sí, false = No, null = Todos.</summary>
        public bool? IsSpecialOrder { get; set; }
    }
}
