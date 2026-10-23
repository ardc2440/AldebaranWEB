namespace Aldebaran.DataAccess.Entities
{
    /// <summary>Fila de SP_GET_ORDER_DOCUMENT_LOG: modificación o cancelación de un despacho o traslado a proceso. Keyless.</summary>
    public class OrderDocumentLogEntry
    {
        public string LogKey { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public int DocumentId { get; set; }
        public DateTime? DocumentDate { get; set; }
        public int CustomerOrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public int TotalRows { get; set; }
    }
}
