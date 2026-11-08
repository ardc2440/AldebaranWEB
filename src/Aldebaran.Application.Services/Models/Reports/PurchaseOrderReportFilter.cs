namespace Aldebaran.Application.Services.Models.Reports
{
    /// <summary>Filtros del reporte de órdenes de compra. Null o vacío = sin filtro.</summary>
    public class PurchaseOrderReportFilter
    {
        public string? OrderNumber { get; set; }
        public DateTime? CreationDateFrom { get; set; }
        public DateTime? CreationDateTo { get; set; }
        public DateTime? RequestDateFrom { get; set; }
        public DateTime? RequestDateTo { get; set; }
        public DateTime? ExpectedReceiptDateFrom { get; set; }
        public DateTime? ExpectedReceiptDateTo { get; set; }
        public string? ReferenceIds { get; set; } // ids separados por coma; vacío = todas
        public string? ImportNumber { get; set; }
        public string? EmbarkationPort { get; set; }
        public string? ProformaNumber { get; set; }
        public int? ProviderId { get; set; }
        public int? ForwarderId { get; set; }
        public int? ForwarderAgentId { get; set; }
        public short? ShipmentMethodId { get; set; }
        public short? WarehouseId { get; set; }
        public short? StatusDocumentTypeId { get; set; }
        public bool RequestedQuantityDifference { get; set; }
    }
}
