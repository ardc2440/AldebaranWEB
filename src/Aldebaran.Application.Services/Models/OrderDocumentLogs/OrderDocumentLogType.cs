namespace Aldebaran.Application.Services.Models.OrderDocumentLogs
{
    /// <summary>Log que se consulta: modificación o cancelación de despachos y de traslados a proceso.</summary>
    public enum OrderDocumentLogType
    {
        ShipmentModification,
        ShipmentCancellation,
        InProcessModification,
        InProcessCancellation
    }
}
