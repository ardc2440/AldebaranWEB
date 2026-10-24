namespace Aldebaran.DataAccess.Entities.Reports
{
    /// <summary>Filtros comunes de los reportes de pedidos (Pedidos, Back order, Actividades y Ventas, con sus exportaciones).</summary>
    public class CustomerOrderReportFilter
    {
        public string? OrderNumber { get; set; }
        public DateTime? CreationDateFrom { get; set; }
        public DateTime? CreationDateTo { get; set; }
        public DateTime? OrderDateFrom { get; set; }
        public DateTime? OrderDateTo { get; set; }
        public DateTime? EstimatedDeliveryDateFrom { get; set; }
        public DateTime? EstimatedDeliveryDateTo { get; set; }
        public short? StatusDocumentTypeId { get; set; }
        public int? CustomerId { get; set; }
        /// <summary>Ids de referencia separados por coma; vacío = todas.</summary>
        public string? ReferenceIds { get; set; }
        /// <summary>Pedido especial: true = Sí, false = No, null = Todos.</summary>
        public bool? IsSpecialOrder { get; set; }
    }
}
