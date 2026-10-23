namespace Aldebaran.DataAccess.Entities
{
    /// <summary>Fila de SP_GET_PURCHASE_ORDER_ADJUSTMENT_LOG: solicitud de aprobación de ajustes de una OC y su resolución. Keyless.</summary>
    public class PurchaseOrderAdjustmentRequestEntry
    {
        public int RequestId { get; set; }
        public DateTime RequestDate { get; set; }
        public int PurchaseOrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string ProviderName { get; set; } = string.Empty;
        public string RequestedBy { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public DateTime? ResolutionDate { get; set; }
        public string? ResolvedBy { get; set; }
        public string? Reason { get; set; }
        public int TotalRows { get; set; }
    }
}
